// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.CodeContracts;

namespace MicroElements.FileStorage.Operations
{
    public class Session : ISession
    {
        private readonly IDataStore _dataStore;
        private readonly IWritableDataStorage _writableStorage;
        private IStorageProvider _storageProvider;
        private List<StoreCommand> _commands = new List<StoreCommand>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="dataStore">The Data store.</param>
        public Session(IDataStore dataStore)
        {
            _dataStore = dataStore;

            if (_dataStore.Configuration.ReadOnly)
                throw new InvalidOperationException("DataStore is readonly and cannot be changed.");

            _writableStorage = _dataStore.GetWritableStorage();
            if (_writableStorage == null)
                throw new InvalidOperationException("DataStore does not contain writable storages.");
        }

        /// <inheritdoc />
        public void AddOrUpdate<T>(T entity, string key = null) where T : class
        {
            key = key ?? GetKey(entity);
            if (key == null)
            {
                key = GetNextKey(entity);
                SetKey(entity, key);
            }

            var sessionCommand = new StoreCommand(CommandType.Store, typeof(T), key)
            {
                Entity = entity
            };

            _commands.Add(sessionCommand);
        }

        /// <inheritdoc />
        public void Delete<T>(string key) where T : class
        {
            Check.NotNull(key, nameof(key));

            var sessionCommand = new StoreCommand(CommandType.Delete, typeof(T), key);

            _commands.Add(sessionCommand);
        }

        /// <inheritdoc />
        public void Patch<T>(string key, IDictionary<string, object> properties) where T : class
        {
            Check.NotNull(key, nameof(key));

            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Drop<T>() where T : class
        {
            //_dataStore.Storages[0].GetEntityList<T>().Clear();
            throw new NotImplementedException();
        }

        private class SaveData
        {
            public Type EntityType { get; set; }
        }

        /// <inheritdoc />
        public void SaveChanges()
        {
            // todo: persist to storage
            // todo: add to storage
            // todo: mark as persisted

            foreach (var storeCommand in _commands)
            {
                if (storeCommand.Persisted)
                    continue;
                _writableStorage.Add(storeCommand);
            }

            var changedTypes = _commands.Select(command => command.EntityType).Distinct().ToArrayTemp();
            foreach (var changedType in changedTypes)
            {
                SaveCollection(new SaveData { EntityType = changedType }).GetAwaiter().GetResult();
            }

            foreach (var storeCommand in _commands)
            {
                // fix transaction
                storeCommand.Persisted = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            SaveChanges();
        }

        private string GetKey<T>(T item) where T : class
        {
            var configurationTyped = _dataStore.GetConfigurationTyped<T>();
            var key = configurationTyped.KeyGetter.GetIdFunc()(item);
            return key;
        }

        private void SetKey<T>(T item, string key) where T : class
        {
            var configurationTyped = _dataStore.GetConfigurationTyped<T>();
            configurationTyped.KeySetter.SetIdFunc()(item, key);
        }

        private string GetNextKey<T>(T item) where T : class
        {
            var collectionConfiguration = _dataStore.GetConfigurationTyped<T>();
            var nextKey = collectionConfiguration.KeyGenerator.GetNextKey(_dataStore, item);
            return nextKey.Formatted;
        }

        private Task SaveCollection(SaveData saveData)
        {
            var methodInfo = GetType()
                .GetMethod(nameof(SaveCollectionInternal), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(saveData.EntityType);

            return (Task)methodInfo.Invoke(this, new[] { saveData });
        }

        private async Task SaveCollectionInternal<T>(SaveData saveData) where T : class
        {
            var configuration = _dataStore.Configuration.GetCollectionConfiguration(saveData.EntityType);
            var serializer = _dataStore.Configuration.GetSerializer(typeof(T));
            var writableStorage = _dataStore.GetWritableStorage();
            var entityList = writableStorage.GetEntityList<T>();
            var serializerInfo = serializer.GetInfo();
            var storeCommands = _commands.Where(command => command.CommandType == CommandType.Store);
            var storeKeys = storeCommands.Select(command => command.Key);

            var collectionDir = configuration.SourceFile;

            if (configuration.IsMultiFile())
            {
                foreach (var storeKey in storeKeys)
                {
                    var item = entityList.Get(storeKey);

                    var format = string.Format("{0}{1}", storeKey, serializerInfo.Extension);
                    var fileName = Path.Combine(collectionDir, format);
                    await SaveFile(serializer, new[] { item }, fileName);
                }

                await ProcessDeletes<T>(collectionDir, serializerInfo.Extension);
            }
            else
            {
                //todo: do not rewrite same
                List<T> items = new List<T>();
                foreach (var key in entityList.Index.AddedKeys)
                {
                    var item = entityList.GetByPos(entityList.Index.KeyPosition[key]);
                    items.Add(item);
                }
                await SaveFile(serializer, items, configuration.SourceFile);
            }
        }

        private async Task SaveFile<T>(ISerializer serializer, IReadOnlyCollection<T> items, string fileName) where T : class
        {
            var fileContent = serializer.Serialize(items, typeof(T));
            var storageProvider = _writableStorage.Configuration.StorageProvider;
            await storageProvider.WriteFile(fileName, fileContent);
        }

        private async Task ProcessDeletes<T>(string collectionDir, string extention) where T : class
        {
            var deleteCommands = _commands.Where(command => !command.Processed && command.CommandType == CommandType.Delete);

            foreach (var deleteCommand in deleteCommands)
            {
                var fileName = Path.Combine(collectionDir, string.Format("{0}{1}", deleteCommand.Key, extention));

                var storageProvider = _writableStorage.Configuration.StorageProvider;
                await storageProvider.DeleteFile(fileName);
                deleteCommand.Processed = true;
            }
        }
    }
}
