// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.CodeContracts;

namespace MicroElements.FileStorage.KeyGenerators
{
    /// <summary>
    /// Identity key generator is like sequence in relational databases.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class IdentityKeyGenerator<T> : IKeyGenerator<T> where T : class
    {
        private readonly int _startValue;
        private readonly bool _useCollectionPrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityKeyGenerator{T}"/> class.
        /// </summary>
        /// <param name="startValue">Start value for id.</param>
        /// <param name="useCollectionPrefix">If true than key formatted as {collectionName}/{id}. if false than key will be {id}.</param>
        public IdentityKeyGenerator(int startValue = 1, bool useCollectionPrefix = true)
        {
            if (startValue < 0)
                throw new ArgumentException("startValue should be positive int value.", nameof(startValue));
            _startValue = startValue;
            _useCollectionPrefix = useCollectionPrefix;
        }

        /// <inheritdoc />
        public KeyType KeyStrategy { get; } = KeyType.Identity;

        /// <inheritdoc />
        public Key GetNextKey(IDataStore dataStore, T entity)
        {
            Check.NotNull(dataStore, nameof(dataStore));

            var collection = dataStore.GetCollection<T>();
            string collectionName = collection.Configuration.Name;

            int ParseKey(string key)
            {
                if (key.Length > collectionName.Length + 1 && key.StartsWith(collectionName))
                {
                    key = key.Substring(collectionName.Length + 1);
                }

                return int.Parse(key);
            }

            int nextId = _startValue;
            if (collection.Count > 0)
            {
                int max = collection
                    .Find(arg => true)
                    .Select(collection.GetKey)
                    .Select(ParseKey)
                    .Max();
                nextId = Math.Max(_startValue, max + 1);
            }

            return new Key(KeyStrategy, nextId.ToString(), _useCollectionPrefix ? collectionName : null);
        }
    }
}
