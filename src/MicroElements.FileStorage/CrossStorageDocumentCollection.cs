// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.Operations;

namespace MicroElements.FileStorage
{
    public class CrossStorageDocumentCollection<T> : IDocumentCollection<T> where T : class
    {
        private readonly IDataStore _dataStore;
        private readonly List<IEntityList<T>> _entityLists;
        private readonly List<IIndex> _indices = new List<IIndex>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CrossStorageDocumentCollection{T}"/> class.
        /// </summary>
        /// <param name="dataStore">The DataStore.</param>
        public CrossStorageDocumentCollection(IDataStore dataStore)
        {
            _dataStore = dataStore;

            _entityLists = new List<IEntityList<T>>();
            foreach (var dataStorage in _dataStore.Storages)
            {
                var entityList = dataStorage.GetEntityList<T>();
                _entityLists.Add(entityList);
                _indices.Add(entityList.Index);
            }
        }

        /// <inheritdoc />
        public CollectionConfiguration Configuration
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public bool HasChanges { get; set; }

        /// <inheritdoc />
        public int Count
        {
            get
            {
                if (_entityLists.Count == 1)
                {
                    return _entityLists[0].Index.AddedKeys.Count;
                }

                var indices = _entityLists.Select(list => list.Index).ToList();
                HashSet<string> added = new HashSet<string>();
                for (int i = indices.Count - 1; i >= 0; i--)
                {
                    added.UnionWith(indices[i].AddedKeys);
                }

                return added.Count;
            }
        }

        /// <inheritdoc />
        public void Drop()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public CollectionConfigurationTyped<T> ConfigurationTyped
        {
            get
            {
                return _dataStore.Configuration
                    .Storages.Last()
                    .Collections
                    .First(configuration => configuration.DocumentType == typeof(T))
                    .ToTyped<T>();
            }
        }

        /// <inheritdoc />
        public void Add(T item)
        {
            using (var session = new Session(_dataStore))
            {
                session.AddOrUpdate(item, null);
            }
        }

        /// <inheritdoc />
        public T Get(string key)
        {
            var indices = _entityLists.Select(list => list.Index).ToList();
            for (int i = indices.Count - 1; i >= 0; i--)
            {
                // if found in added
                if (indices[i].KeyPosition.TryGetValue(key, out var pos))
                {
                    var entity = _entityLists[i].GetByPos(pos);
                    if (entity != null)
                    {
                        // entity found
                        return entity;
                    }

                    if (indices[i].DeletedKeys.Contains(key))
                    {
                        // entity was deleted
                        return null;
                    }
                }
            }

            return null;
        }

        /// <inheritdoc />
        public bool IsExists(string key)
        {
            var indices = _entityLists.Select(list => list.Index).ToList();
            for (int i = indices.Count - 1; i >= 0; i--)
            {
                if (indices[i].AddedKeys.Contains(key))
                    return true;

                if (indices[i].DeletedKeys.Contains(key))
                    return false;
            }

            return false;
        }

        /// <inheritdoc />
        public IEnumerable<T> Find(Func<T, bool> query)
        {
            var indices = _entityLists.Select(list => list.Index).ToList();
            var fullIndex = IndexBuilder.BuildFullIndex(indices, _entityLists);
            var result = fullIndex.Values
                .Select(indexKey => indexKey.EntityList.GetByPos(indexKey.Pos))
                .Where(query);
            return result;
        }

        /// <inheritdoc />
        public void Delete(string key)
        {
            using (var session = new Session(_dataStore))
            {
                session.Delete<T>(key);
            }
        }

        /// <inheritdoc />
        public IReadOnlyCommandLog GetCommandLog()
        {
            throw new NotImplementedException();
        }
    }
}
