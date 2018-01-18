// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.CodeContracts;

namespace MicroElements.FileStorage
{
    /// <summary>
    /// Typed document collection.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class DocumentCollection<T> : IDocumentCollection<T> where T : class
    {
        private readonly List<T> _documents = new List<T>();
        private readonly Dictionary<string, int> _indexIdDocIndex = new Dictionary<string, int>();

        private readonly IKeyGetter<T> _keyGetter;
        private readonly IKeySetter<T> _keySetter;

        public DocumentCollection(CollectionConfiguration configuration)
        {
            Configuration = configuration;

            //todo: передавать снаружи
            ConfigurationTyped = (configuration as CollectionConfigurationTyped<T>) ?? new CollectionConfigurationTyped<T>();
            _keyGetter = ConfigurationTyped.KeyGetter;
            _keySetter = ConfigurationTyped.KeySetter;
        }

        /// <inheritdoc />
        public CollectionConfiguration Configuration { get; }

        /// <inheritdoc />
        public CollectionConfigurationTyped<T> ConfigurationTyped { get; }

        /// <inheritdoc />
        public bool HasChanges { get; set; }

        /// <inheritdoc />
        public int Count
        {
            get
            {
                lock (_documents)
                {
                    return _documents.Count;
                }
            }
        }

        /// <inheritdoc />
        public void Add(T item)
        {
            Check.NotNull(item, nameof(item));

            lock (_documents)
            {
                var key = GetKey(item);
                if (key == null)
                {
                    key = GetNextKey(item);
                    SetKey(item, key);
                }

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

                HasChanges = true;
            }
        }

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

                return null;
            }
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
        public IEnumerable<T> Find(Func<T, bool> query)
        {
            lock (_documents)
            {
                return _documents.Where(query);
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
                    _documents.Remove(entity);
                    _indexIdDocIndex.Remove(key);
                    // todo: delete is not deletes from file storage
                    // _storageEngine.Delete(key);
                }
            }
            else
            {
                // todo: error?
                // throw new EntityNotFoundException();
            }
        }

        /// <inheritdoc />
        public void Drop()
        {
            lock (_documents)
            {
                // todo: delete is not deletes from file storage
                _documents.Clear();
                _indexIdDocIndex.Clear();
            }
        }

        private string GetKey(T item) => _keyGetter.GetIdFunc()(item);

        private void SetKey(T item, string key) => _keySetter.SetIdFunc()(item, key);

        private string GetNextKey(T item)
        {
            var nextKey = ConfigurationTyped.KeyGenerator.GetNextKey(this, item);
            return nextKey.Formatted;
        }
    }
}
