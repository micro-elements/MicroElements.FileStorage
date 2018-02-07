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
        private IDataStore _dataStore;
        private IStorageProvider _storageProvider;
        private List<StoreCommand> _commands = new List<StoreCommand>();

        /// <inheritdoc />
        public Session(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            SaveChanges();
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
            //var dataLoader = new DataLoader(_dataStore, _dataStore.Storages.Last().Configuration);
            //dataLoader.SaveCollection();

            _dataStore.Save();

            //_entityLists.Last().AddOrUpdate();
            //_storageProvider.WriteFile()
            //throw new NotImplementedException();
        }
    }
}
