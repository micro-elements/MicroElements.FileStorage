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
        public bool HasChanges { get; private set; }

        /// <inheritdoc />
        public int Count => _documents.Count;

        /// <inheritdoc />
        public IReadOnlyCollection<object> GetAll() => _documents;

        /// <inheritdoc />
        public void Drop()
        {
            _documents.Clear();
        }

        /// <inheritdoc />
        public void Add(T item)
        {
            Check.NotNull(item, nameof(item));

            //todo: ThreadSafe
            var key = GetKey(item);
            if (key == null)
            {
                key = GetNextKey();
                SetKey(item, key);
            }
            _documents.Add(item);
            HasChanges = true;
        }

        private string GetNextKey()
        {
            var nextKey = ConfigurationTyped.KeyGenerator.GetNextKey(this);
            return nextKey.Formatted;
        }

        private string GetKey(T item) => _keyGetter.GetIdFunc()(item);

        private void SetKey(T item, string key) => _keySetter.SetIdFunc()(item, key);

        /// <inheritdoc />
        public T Get(string key)
        {
            Check.NotNull(key, nameof(key));

            return _documents.FirstOrDefault(arg => _keyGetter.GetIdFunc()(arg) == key);
        }

        /// <inheritdoc />
        public IEnumerable<T> Find(Func<T, bool> query)
        {
            return _documents.Where(query);
        }
    }
}