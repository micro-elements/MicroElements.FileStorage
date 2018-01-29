// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.Utils;

namespace MicroElements.FileStorage.KeyGenerators
{
    public class HashKeyGenerator<T> : IKeyGenerator<T> where T : class
    {
        private readonly ISerializer _serializer;

        public HashKeyGenerator(ISerializer serializer)
        {
            _serializer = serializer;
        }

        /// <inheritdoc />
        public KeyType KeyStrategy { get; } = KeyType.Hash;

        /// <inheritdoc />
        public Key GetNextKey(IDocumentCollection<T> collection, T entity)
        {
            var key = entity.Md5Hash(_serializer);
            return new Key(KeyType.Hash, key);
        }
    }
}
