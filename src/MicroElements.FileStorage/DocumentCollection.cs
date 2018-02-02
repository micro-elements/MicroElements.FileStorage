// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.CodeContracts;
using MicroElements.FileStorage.Operations;

namespace MicroElements.FileStorage
{
    /// <summary>
    /// Typed document collection.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class DocumentCollection<T> : IDocumentCollection<T> where T : class
    {
        private readonly List<T> _documents = new List<T>();
        private readonly ConcurrentDictionary<string, int> _indexIdDocIndex = new ConcurrentDictionary<string, int>();
        private readonly IDocumentContainer<T> _documentContainer = new DocumentContainer<T>();
        private readonly CommandLog _commandLog = new CommandLog();

        private readonly IKeyGetter<T> _keyGetter;
        private readonly IKeySetter<T> _keySetter;

        public DocumentCollection(CollectionConfiguration configuration)
        {
            Configuration = configuration;

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
        public int Count => _indexIdDocIndex.Count;

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

                // Validation.
                var validator = GetValidator();
                if (validator != null)
                {
                    var validationResult = validator.Validate(item);
                    if (!validationResult.IsValid)
                        throw new ValidationException(validationResult.Errors);
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
                return _documents
                    .Where(d => d != null)
                    .Where(query);
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

                _commandLog.Add(new StoreCommand(CommandType.Delete, typeof(T), key));
                HasChanges = true;
            }
            else
            {
                // todo: error?
                // throw new EntityNotFoundException();
            }
        }

        /// <inheritdoc />
        public IReadOnlyCommandLog GetCommandLog()
        {
            return _commandLog;
        }

        /// <inheritdoc />
        public void Drop()
        {
            lock (_documents)
            {
                foreach (var key in _indexIdDocIndex.Keys)
                {
                    _commandLog.Add(new StoreCommand(CommandType.Delete, typeof(T), key));
                }

                _documents.Clear();
                _indexIdDocIndex.Clear();
            }
        }

        /// <summary>
        /// Validates all loaded entities.
        /// </summary>
        /// <returns>Aggregated validation result.</returns>
        public IDictionary<object, ValidationResult> CheckAll()
        {
            IDictionary<object, ValidationResult> validationResult = new Dictionary<object, ValidationResult>();
            IValidator<T> validator = GetValidator();
            if (validator == null)
                return validationResult;

            foreach (var document in _documents)
            {
                var result = validator.Validate(document);
                if (!result.IsValid)
                {
                    validationResult.Add(document, result);
                }
            }

            return validationResult;
        }

        private string GetKey(T item) => _keyGetter.GetIdFunc()(item);

        private void SetKey(T item, string key) => _keySetter.SetIdFunc()(item, key);

        private string GetNextKey(T item)
        {
            var nextKey = ConfigurationTyped.KeyGenerator.GetNextKey(this, item);
            return nextKey.Formatted;
        }

        private IValidator<T> GetValidator()
        {
            return ConfigurationTyped.ValidatorFactory?.GetValidator<T>();
        }
    }
}
