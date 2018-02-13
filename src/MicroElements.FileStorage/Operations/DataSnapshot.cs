// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.Operations
{
    public class DataSnapshot : IDataStorage
    {
        private readonly IDataStore _dataStore;
        private readonly DataStorageConfiguration _configuration;

        private readonly IDictionary<Type, IEntityList> _entityLists = new Dictionary<Type, IEntityList>();

        public DataSnapshot(IDataStore dataStore, DataStorageConfiguration configuration)
        {
            _dataStore = dataStore;
            _configuration = configuration;
        }

        /// <inheritdoc />
        public IDataStorageConfiguration Configuration => _configuration;

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

        /// <inheritdoc />
        public IEntityList<T> GetEntityList<T>() where T : class
        {
            return _entityLists.TryGetValue(typeof(T), out var list) ?
                (IEntityList<T>)list :
                throw new InvalidOperationException($"EntityList for type {typeof(T)} is not registered in storage.");
        }
    }
}
