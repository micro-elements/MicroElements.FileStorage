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
    public class DataSnapshot : IDataSnapshot
    {
        private IDataStore _dataStore;
        private readonly List<IDocumentCollection> _collections = new List<IDocumentCollection>();
        private ILoggerFactory _loggerFactory;
        private DataStorageConfiguration _configuration;

        /// <inheritdoc />
        public DataSnapshot(IDataStore dataStore, ILoggerFactory loggerFactory, DataStorageConfiguration configuration)
        {
            _dataStore = dataStore;
            _loggerFactory = loggerFactory;
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
        public IReadOnlyList<IDocumentCollection> GetCollections()
        {
            return _collections.AsReadOnly();
        }

        /// <inheritdoc />
        public async Task Initialize()
        {
            var dataLoader = new DataLoader(
                _dataStore.Configuration.Conventions,
                _loggerFactory.CreateLogger(typeof(DataLoader)),
                _configuration);
            var collections = await dataLoader.LoadCollectionsAsync(_configuration);
            _collections.AddRange(collections);
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
            var dataLoader = new DataLoader(
                _dataStore.Configuration.Conventions,
                _loggerFactory.CreateLogger(typeof(DataLoader)),
                _configuration);
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
