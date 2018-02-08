// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.Operations
{
    public class Session : ISession
    {
        private readonly IDataStore _dataStore;
        private readonly IDataAddon _writableStorage;
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
                TimestampUtc = DateTime.UtcNow,
                Entity = entity
            };

            _commands.Add(sessionCommand);
        }

        /// <inheritdoc />
        public void Delete<T>(string key) where T : class
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Patch<T>(string key, IDictionary<string, object> properties) where T : class
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void SaveChanges()
        {
            //_commands.GroupBy(command => command.EntityType)
            foreach (var storeCommand in _commands)
            {
                // todo: persist to storage
                // todo: add to in memory addon
                // todo: mark as persisted

                _writableStorage.Add(storeCommand);

                storeCommand.Persisted = true;
            }

            //_writableStorage.Add(_commands[0]);
            //var storageProvider = _writableStorage.Configuration.StorageProvider;
            //storageProvider.WriteFile()
            //var dataLoader = new DataLoader(_dataStore, _dataStore.Storages.Last().Configuration);
            //dataLoader.SaveCollection();

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
    }
}
