// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MicroElements.FileStorage.Abstractions;

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
        public void AddOrUpdate<T>(T entity, string key) where T : class
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
            var sessionCommand = new StoreCommand(CommandType.Delete, typeof(T), key);

            _commands.Add(sessionCommand);
        }

        /// <inheritdoc />
        public void Patch<T>(string key, IDictionary<string, object> properties) where T : class
        {
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
            public Type EntityType;
            public IEntityList EntityList;
            public List<StoreCommand> Commands;
            public CollectionConfiguration CollectionConfiguration;
        }

        /// <inheritdoc />
        public void SaveChanges()
        {
            //_commands.GroupBy(command => command.EntityType)
            var storeCommands = _commands.Where(command => command.CommandType == CommandType.Store);
            var deleteCommands = _commands.Where(command => command.CommandType == CommandType.Delete);

            foreach (var storeCommand in _commands)
            {
                // todo: persist to storage
                // todo: add to in memory addon
                // todo: mark as persisted

                _writableStorage.Add(storeCommand);

                storeCommand.Persisted = true;
            }
            var changedTypes = _commands.Select(command => command.EntityType).Distinct().ToArrayTemp();
            foreach (var changedType in changedTypes)
            {
                _writableStorage.GetEntityList<object>();
                _dataStore.GetCollection<object>().Find()
            }

            //_writableStorage.Add(_commands[0]);
            var storageProvider = _writableStorage.Configuration.StorageProvider;
            storageProvider.WriteFile()
            var dataLoader = new DataLoader(_dataStore, _dataStore.Storages.Last().Configuration);
            dataLoader.SaveCollection();

            //var dataLoader = new DataLoader(_dataStore, _writableStorage.Configuration);
            //var collections = _dataStore.GetCollections();
            //foreach (var collection in collections)
            //{
            //    if (collection.HasChanges)
            //    {
            //        dataLoader.SaveCollection(collection);
            //    }
            //}
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

        private void SaveCollection(SaveData saveData)
        {
            GetType().GetMethod(nameof(SaveCollectionInternal), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(saveData.EntityType)
                .Invoke(this, new[] { saveData });
        }

        private void SaveCollectionInternal<T>(SaveData saveData) where T : class
        {
            var configuration = _dataStore.Configuration.GetCollectionConfiguration(saveData.EntityType);
            var serializer = _dataStore.Configuration.GetSerializer(typeof(T));
            var serializerInfo = serializer.GetInfo();
            var items = collection.Find(arg => true).ToList();
            var collectionDir = configuration.SourceFile;

            if (IsMultiFile(configuration))
            {
                foreach (var item in items)
                {
                    var key = collection.GetKey(item);
                    var format = string.Format("{0}{1}", key, serializerInfo.Extension);
                    var fileName = Path.Combine(collectionDir, format);
                    SaveFiles(serializer, new[] { item }, fileName, configuration.DocumentType);
                }

                ProcessDeletes(collection, collectionDir, serializerInfo.Extension).GetAwaiter().GetResult();
            }
            else
            {
                //todo: do not rewrite same
                SaveFiles(serializer, items, configuration.SourceFile, configuration.DocumentType);
            }

            collection.HasChanges = false;
        }
    }



    public class DataSaver
    {

    }
}
