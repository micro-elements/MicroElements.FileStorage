// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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
            _dataStore.Save();
        }

        /// <inheritdoc />
        public void AddOrUpdate<T>(T entity, string key) where T : class
        {
            _dataStore.GetWriteProvider();
            var sessionCommand = new StoreCommand(CommandType.Store, typeof(T), key, null);
            sessionCommand.TimestampUtc = DateTime.UtcNow;
            var configuration = _dataStore.GetCollection<T>().ConfigurationTyped;
            var serializerInfo = configuration.Serializer.GetInfo();

            // todo: serializer is not optimal for one object
            sessionCommand.Content = configuration.Serializer.Serialize(new[] { entity }, typeof(T)).Content;

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
            //_storageProvider.WriteFile()
            throw new NotImplementedException();
        }
    }
}
