// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.CodeContracts;
using MicroElements.FileStorage.Utils;

namespace MicroElements.FileStorage.KeyGenerators
{
    /// <summary>
    /// Generates key as MD5 hash value from serialized value.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class HashKeyGenerator<T> : IKeyGenerator<T> where T : class
    {
        private readonly ISerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashKeyGenerator{T}"/> class.
        /// </summary>
        /// <param name="serializer">Serializer to get entity in serialized form.</param>
        public HashKeyGenerator(ISerializer serializer)
        {
            Check.NotNull(serializer, nameof(serializer));
            _serializer = serializer;
        }

        /// <inheritdoc />
        public KeyType KeyStrategy { get; } = KeyType.Hash;

        /// <inheritdoc />
        public Key GetNextKey(IDataStore dataStore, T entity)
        {
            Check.NotNull(entity, nameof(entity));

            var key = entity.Md5Hash(_serializer);
            return new Key(KeyStrategy, key);
        }
    }
}
