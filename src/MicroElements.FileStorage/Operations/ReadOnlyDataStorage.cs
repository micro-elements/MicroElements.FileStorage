// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.Operations
{
    /// <summary>
    /// ReadOnly Data Storage.
    /// </summary>
    public class ReadOnlyDataStorage : IDataStorage
    {
        private readonly IDataStore _dataStore;
        private readonly DataStorageConfiguration _configuration;

        private readonly IDictionary<Type, IEntityList> _entityLists = new Dictionary<Type, IEntityList>();

        public ReadOnlyDataStorage(IDataStore dataStore, DataStorageConfiguration configuration)
        {
            _dataStore = dataStore;
            _configuration = configuration;
        }

        /// <inheritdoc />
        public IDataStorageConfiguration Configuration => _configuration;

        /// <inheritdoc />
        public async Task Initialize()
        {
            var dataLoader = new FileSystemLoader(new LoaderSettings(_dataStore, _configuration));
            var collectionDatas = await dataLoader.LoadEntitiesAsync();

            foreach (var valuePair in collectionDatas)
            {
                var entityList = EntityListFactory.Create(typeof(ReadOnlyEntityList<>), valuePair.Value);
                _entityLists.Add(valuePair.Key, entityList);
            }
        }

        /// <inheritdoc />
        public IEntityList GetEntityList(Type entityType)
        {
            return _entityLists.TryGetValue(entityType, out var list) ? list : null;
        }

        /// <inheritdoc />
        public IEntityList<T> GetEntityList<T>() where T : class
        {
            return (IEntityList<T>)GetEntityList(typeof(T));
        }
    }
}
