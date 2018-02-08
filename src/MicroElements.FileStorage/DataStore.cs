// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.Abstractions.Exceptions;
using MicroElements.FileStorage.Operations;
using Microsoft.Extensions.Logging;

namespace MicroElements.FileStorage
{
    public class DataStore : IDataStore
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly DataStoreConfiguration _configuration;
        private IImmutableList<IDataStorage> _dataStorages = ImmutableArray<IDataStorage>.Empty;
        private IImmutableDictionary<Type, IDocumentCollection> _collections = ImmutableDictionary<Type, IDocumentCollection>.Empty;
        private Schema _schema;

        public DataStore(DataStoreConfiguration configuration)
        {
            _configuration = configuration;
            _loggerFactory = configuration.LoggerFactory;
            Services = new DataStoreServices(configuration.LoggerFactory);
        }

        /// <inheritdoc />
        public DataStoreConfiguration Configuration => _configuration;

        /// <inheritdoc />
        public IReadOnlyList<IDataStorage> Storages => _dataStorages;

        /// <inheritdoc />
        public Schema Schema => _schema;

        /// <inheritdoc />
        public DataStoreServices Services { get; }

        /// <inheritdoc />
        public async Task Initialize()
        {
            if (_schema != null)
                throw new FileStorageException("DataStore already initialized");

            _configuration.Verify();
            _schema = new Schema(_configuration);

            foreach (var configurationStorage in _configuration.Storages)
            {
                var dataStorage = new DataSnapshot(this, configurationStorage);
                await dataStorage.Initialize();
                _dataStorages = _dataStorages.Add(dataStorage);
            }

            if (!_configuration.ReadOnly)
            {
                var countOfWritableStorages = Storages.Count(storage => storage.IsWritable());
                if (countOfWritableStorages > 1)
                    throw new InvalidConfigurationException("DataStore is writable but there more than one writable storages.");

                if (_dataStorages.Count == 1 || countOfWritableStorages == 0)
                {
                    _dataStorages = _dataStorages.Add(new DataAddon());
                }
            }

            foreach (var documentType in _schema.DocumentTypes)
            {
                var documentCollection = CreateCollection(documentType);
                _collections = _collections.Add(documentType, documentCollection);
            }
        }

        private IDataStorage DataStorage => _dataStorages.Last();

        /// <inheritdoc />
        public IDocumentCollection<T> GetCollection<T>() where T : class
        {
            return _collections.TryGetValue(typeof(T), out var collection) ?
                (IDocumentCollection<T>)collection :
                throw new InvalidOperationException($"Document collection for type {typeof(T)} is not registered in data store.");
        }

        /// <inheritdoc />
        public IReadOnlyList<IDocumentCollection> GetCollections()
        {
            return _collections.Values.ToList();
        }

        /// <inheritdoc />
        public void Save()
        {
            DataStorage.Save();
        }

        /// <inheritdoc />
        public void Drop()
        {
            DataStorage.Drop();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            (_configuration.StorageProvider as IDisposable)?.Dispose();
        }

        /// <inheritdoc />
        public ISession OpenSession()
        {
            return new Session(this);
        }

        private IDocumentCollection CreateCollection(Type documentType)
        {
            //todo: move to factory
            return (IDocumentCollection)Activator.CreateInstance(typeof(CrossStorageDocumentCollection<>).MakeGenericType(documentType), this);

            if (_dataStorages.Count > 1)
            {
                var documentCollectionType = typeof(CrossStorageDocumentCollection<>).MakeGenericType(documentType);
                return (IDocumentCollection)Activator.CreateInstance(documentCollectionType, this);
            }
            else
            {
                var documentCollectionType = typeof(DocumentCollection<>).MakeGenericType(documentType);
                var collectionConfiguration = _dataStorages[0].Configuration.Collections
                    .First(configuration => configuration.DocumentType == documentType);
                return (IDocumentCollection)Activator.CreateInstance(documentCollectionType, collectionConfiguration);
            }
        }
    }
}
