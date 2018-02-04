// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.Operations
{
    public class StoreCommand
    {
        /// <inheritdoc />
        public StoreCommand(CommandType commandType, Type entityType, string key = null, string content = null)
        {
            TimestampUtc = DateTime.UtcNow;
            CommandType = commandType;
            EntityType = entityType;

            Key = key;
            Content = content;
        }

        public DateTime TimestampUtc { get; set; }
        public CommandType CommandType { get; set; }
        public Type EntityType { get; set; }
        public string Key { get; set; }
        public string Content { get; set; }
        public object Entity { get; set; }

        public bool Processed { get; set; }
        public bool Persisted { get; set; }

        public StoreMetadata Metadata { get; set; }
    }

    public class StoreMetadata
    {
        public string Serializer { get; set; }
        public string Format { get; set; }
        public string Version { get; set; }
    }

    internal class SnapshotInfo
    {
        public DateTime Timestamp { get; set; }
    }

    public class DataAddon : IDataAddon
    {
        private List<StoreCommand> _commands = new List<StoreCommand>();

        /// <inheritdoc />
        public void Add(StoreCommand command)
        {
            _commands.Add(command);
        }

        /// <inheritdoc />
        public Task Initialize()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IDocumentCollection<T> GetCollection<T>() where T : class
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IEntityList<T> GetDocList<T>() where T : class
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IReadOnlyList<Type> GetDocTypes()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IReadOnlyList<IDocumentCollection> GetCollections()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Drop()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
