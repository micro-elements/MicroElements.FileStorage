// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.CodeContracts;

namespace MicroElements.FileStorage
{
    public class WritableEntityList<T> : IEntityList<T>, IEnumerable<T> where T : class
    {
        private readonly List<T> _documents = new List<T>();
        private readonly ConcurrentDictionary<string, int> _indexIdDocIndex = new ConcurrentDictionary<string, int>();

        private IIndex<string> _byKeyIndex;

        /// <inheritdoc />
        public bool IsReadOnly { get; }

        /// <inheritdoc />
        public T Get(string key)
        {
            Check.NotNull(key, nameof(key));

            lock (_documents)
            {
                if (_indexIdDocIndex.TryGetValue(key, out int index))
                {
                    return _documents[index];
                }
            }

            return null;
        }

        /// <inheritdoc />
        public bool IsExists(string key)
        {
            Check.NotNull(key, nameof(key));

            lock (_documents)
            {
                return _indexIdDocIndex.ContainsKey(key);
            }
        }

        /// <inheritdoc />
        public void AddOrUpdate(T item, string key)
        {
            Check.NotNull(item, nameof(item));
            Check.NotNull(key, nameof(key));

            lock (_documents)
            {
                if (_indexIdDocIndex.TryGetValue(key, out int index))
                {
                    // Update item.
                    _documents[index] = item;
                }
                else
                {
                    // Add item.
                    _documents.Add(item);
                    _indexIdDocIndex[key] = _documents.Count - 1;
                }
            }
        }

        /// <inheritdoc />
        public void Delete(string key)
        {
            var entity = Get(key);
            if (entity != null)
            {
                lock (_documents)
                {
                    if (_indexIdDocIndex.TryGetValue(key, out int index))
                    {
                        _documents[index] = null;
                    }

                    _indexIdDocIndex.TryRemove(key, out _);
                }
            }
            else
            {
                // todo: error?
                // throw new EntityNotFoundException();
            }
        }

        public IEnumerable<T> Find(Func<T, bool> query)
        {
            lock (_documents)
            {
                return _documents
                    .Where(d => d != null)
                    .Where(query);
            }
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return _documents.Where(d => d != null).GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
