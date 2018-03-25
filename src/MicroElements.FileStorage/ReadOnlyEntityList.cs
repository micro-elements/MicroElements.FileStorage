// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage
{
    /// <summary>
    /// ReadOnly document collection.
    /// <para>No need of syncronization.</para>
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class ReadOnlyEntityList<T> : IEntityList<T> where T : class
    {
        private readonly T[] _documents;
        private readonly Dictionary<string, int> _indexKeyPosition;
        private readonly HashSet<string> _deleted;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyEntityList{T}"/> class.
        /// </summary>
        /// <param name="data">Data exported from other collection.</param>
        public ReadOnlyEntityList(ExportData<T> data)
        {
            var documents = new T[data.Added.Count];
            Dictionary<string, int> indexKeyPosition = new Dictionary<string, int>();

            for (var index = 0; index < data.Added.Count; index++)
            {
                var entityWithKey = data.Added[index];
                documents[index] = entityWithKey.Entity;
                indexKeyPosition[entityWithKey.Key] = index;
            }

            _documents = documents;
            _indexKeyPosition = indexKeyPosition;
            _deleted = new HashSet<string>(data.Deleted ?? Array.Empty<string>());
        }

        public ReadOnlyEntityList(CollectionData data)
        {
            // todo: validate data
            _documents = new T[data.Entities.Length];

            Dictionary<string, int> indexKeyPosition = new Dictionary<string, int>();

            for (var index = 0; index < data.Entities.Length; index++)
            {
                var entity = data.Entities[index];
                var key = data.Keys[index];
                _documents[index] = (T)entity;
                indexKeyPosition[key] = index;
            }

            _indexKeyPosition = indexKeyPosition;
            _deleted = new HashSet<string>(data.DeletedKeys ?? Array.Empty<string>());
        }

        /// <inheritdoc />
        public Type EntityType => typeof(T);

        /// <inheritdoc />
        public int Count => Index.AddedKeys.Count;

        /// <inheritdoc />
        public T Get(string key)
        {
            return IsDeleted(key) ? null : (_indexKeyPosition.TryGetValue(key, out var pos) ? _documents[pos] : null);
        }

        /// <inheritdoc />
        public T GetByPos(int pos)
        {
            if (pos >= 0 && pos < _documents.Length)
                return _documents[pos];
            return null;
        }

        /// <inheritdoc />
        public bool IsExists(string key)
        {
            return !IsDeleted(key) && _indexKeyPosition.ContainsKey(key);
        }

        /// <inheritdoc />
        public bool IsReadOnly => true;

        private bool IsDeleted(string key)
        {
            return _deleted != null && _deleted.Contains(key);
        }

        /// <inheritdoc />
        public void AddOrUpdate(T item, string key)
        {
            throw new InvalidOperationException("Collection is read only");
        }

        /// <inheritdoc />
        public void Delete(string key)
        {
            throw new InvalidOperationException("Collection is read only");
        }

        /// <inheritdoc />
        public void Clear()
        {
            throw new InvalidOperationException("Collection is read only");
        }

        /// <inheritdoc />
        public IIndex Index => new Index(_indexKeyPosition, _deleted);
    }
}
