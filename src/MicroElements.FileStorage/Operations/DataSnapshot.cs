// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;
using Microsoft.Extensions.Logging;

namespace MicroElements.FileStorage.Operations
{
    public class DataSnapshot : IDataStorage
    {
        private readonly IDataStore _dataStore;
        private readonly ILoggerFactory _loggerFactory;
        private readonly DataStorageConfiguration _configuration;

        // todo: replace collection for container
        private readonly List<IDocumentCollection> _collections = new List<IDocumentCollection>();
        private readonly IDictionary<Type, IEntityList> _entityLists = new Dictionary<Type, IEntityList>();

        /// <inheritdoc />
        public DataSnapshot(IDataStore dataStore, DataStorageConfiguration configuration)
        {
            _dataStore = dataStore;
            _loggerFactory = dataStore.Services.LoggerFactory;
            _configuration = configuration;
        }

        /// <inheritdoc />
        public IDocumentCollection<T> GetCollection<T>() where T : class
        {
            var documentCollection = (IDocumentCollection<T>)_collections.FirstOrDefault(collection => collection is IDocumentCollection<T>);
            // todo: add snapshot key to message
            return documentCollection ?? throw new InvalidOperationException($"Document collection for type {typeof(T)} is not registered in data store.");
        }

        /// <inheritdoc />
        public IEntityList<T> GetEntityList<T>() where T : class
        {
            return _entityLists.TryGetValue(typeof(T), out var list) ?
                (IEntityList<T>)list :
                throw new InvalidOperationException($"EntityList for type {typeof(T)} is not registered in storage.");
        }

        /// <inheritdoc />
        public IReadOnlyList<Type> GetDocTypes()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IReadOnlyList<IDocumentCollection> GetCollections()
        {
            return _collections.AsReadOnly();
        }

        /// <inheritdoc />
        public async Task Initialize()
        {
            var dataLoader = new DataLoader(_dataStore, _configuration);
            var entityLists = await dataLoader.LoadEntitiesAsync();

            foreach (var entityList in entityLists)
            {
                _entityLists.Add(entityList);
            }
        }

        public void Drop()
        {
            foreach (var collection in _collections)
            {
                collection.Drop();
            }
        }

        public void Save()
        {
            var dataLoader = new DataLoader(_dataStore, _configuration);
            foreach (var collection in _collections)
            {
                if (collection.HasChanges)
                {
                    dataLoader.SaveCollection(collection);
                }
            }
        }

    }
}
