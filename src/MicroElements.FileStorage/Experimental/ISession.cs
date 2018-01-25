// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.Experimental
{
    public interface ISession : IDisposable
    {
        void Store<T>(T entity) where T : class;

        void Delete<T>(string key) where T : class;

        void Patch<T>(string key, IDictionary<string, object> properties) where T : class;
    }

    public class SessionCommand
    {
        public DateTime TimestampUtc { get; set; }
        public Command Command { get; set; }
        public string Serializer { get; set; }
        public string Format { get; set; }
        public string Version { get; set; }
        public string Content { get; set; }


    }

    public class Session : ISession
    {
        private IDataStore _dataStore;
        private IStorageEngine _storageEngine;

        private List<SessionCommand> _commands = new List<SessionCommand>();

        /// <inheritdoc />
        public void Dispose()
        {
            _dataStore.Save();
        }

        /// <inheritdoc />
        public void Store<T>(T entity) where T : class
        {
            var sessionCommand = new SessionCommand();
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

    public class SessionLog
    {

    }

    /// <summary>
    /// Data snapshot represents data state in time.
    /// </summary>
    internal interface IDataSnapshot
    {
    }

    internal class SnapshotInfo
    {
        public DateTime Timestamp { get; set; }
    }

    internal class DataAddon
    {
        public DataCommand[] Rows { get; set; }
    }

    public class DataCommand
    {
        public DateTime TimestampUtc { get; set; }
        public Command Command { get; set; }
        public FileContent Content { get; set; }
    }

    public enum Command
    {
        Store,
        Patch,
        Delete,
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
