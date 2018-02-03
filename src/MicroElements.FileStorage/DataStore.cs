// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.Operations;
using Microsoft.Extensions.Logging;

namespace MicroElements.FileStorage
{
    public class DataStore : IDataStore
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly DataStoreConfiguration _configuration;
        private IDataSnapshot _dataStorage;

        public DataStore(DataStoreConfiguration configuration)
        {
            _configuration = configuration;
            _loggerFactory = configuration.LoggerFactory;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            (_configuration.StorageProvider as IDisposable)?.Dispose();
        }

        public async Task Initialize()
        {
            _configuration.Verify();

            var dataStorageConfiguration = _configuration.Storages.First();

            _dataStorage = new DataSnapshot(this, _loggerFactory, dataStorageConfiguration);
            await _dataStorage.Initialize();
        }

        /// <inheritdoc />
        public IDocumentCollection<T> GetCollection<T>() where T : class
        {
            return _dataStorage.GetCollection<T>();
        }

        /// <inheritdoc />
        public IReadOnlyList<IDocumentCollection> GetCollections()
        {
            return _dataStorage.GetCollections();
        }

        /// <inheritdoc />
        public void Save()
        {
            _dataStorage.Save();
        }

        /// <inheritdoc />
        public void Drop()
        {
            _dataStorage.Drop();
        }

        /// <inheritdoc />
        public DataStoreConfiguration Configuration => _configuration;

        /// <inheritdoc />
        public ISession OpenSession()
        {
            return new Session(this);
        }
    }
}
