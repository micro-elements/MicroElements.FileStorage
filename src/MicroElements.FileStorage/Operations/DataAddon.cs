// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.Operations
{
    public class DataAddon : IDataAddon
    {
        private readonly IDataStore _dataStore;
        private readonly DataStorageConfiguration _configuration;
        private readonly DataSnapshot _dataSnapshot;
        private List<StoreCommand> _commands = new List<StoreCommand>();


        public DataAddon(IDataStore dataStore, DataStorageConfiguration configuration)
        {
            _dataStore = dataStore;
            _configuration = configuration;
            _dataSnapshot = new DataSnapshot(dataStore, configuration);
        }

        /// <inheritdoc />
        public void Add(StoreCommand command)
        {
            _commands.Add(command);
        }

        /// <inheritdoc />
        public async Task Initialize()
        {
            await _dataSnapshot.Initialize();
        }

        /// <inheritdoc />
        public IDocumentCollection<T> GetCollection<T>() where T : class
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IEntityList<T> GetEntityList<T>() where T : class
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IReadOnlyList<Type> GetDocTypes()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IDataStorageConfiguration Configuration => _configuration;

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
