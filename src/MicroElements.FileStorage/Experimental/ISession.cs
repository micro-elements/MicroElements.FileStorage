// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.Operations;

namespace MicroElements.FileStorage.Experimental
{
    public interface ISession : IDisposable
    {
        void Store<T>(T entity) where T : class;

        void Delete<T>(string key) where T : class;

        void Patch<T>(string key, IDictionary<string, object> properties) where T : class;
    }

    public class Session : ISession
    {
        private IDataStore _dataStore;
        private IStorageProvider _storageProvider;

        private List<StoreCommand> _commands = new List<StoreCommand>();

        /// <inheritdoc />
        public void Dispose()
        {
            _dataStore.Save();
        }

        /// <inheritdoc />
        public void Store<T>(T entity) where T : class
        {
            var sessionCommand = new StoreCommand(CommandType.Store, typeof(T), null, null);
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
    }

    // https://github.com/micro-elements/MicroElements.FileStorage/issues/6
    // GH #6 Issue

    /*
proposal:

* Snapshot is original data
* Addon is snapshot changes: add, update, delete
* It can be many addons
* Addons must be ordered
* Snapshot+addon_1+...+addon_N = new snapshot (compaction)
* 
* Audit: initiator, timestamp, signature
* Bulk?
* Import/Export?
* Backup
* Append only storage?

    */
}
