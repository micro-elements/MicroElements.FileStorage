// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.KeyGenerators
{
    /// <summary>
    /// Generates Guid keys.
    /// </summary>
    /// <remarks>Note: Guid keys is globally unique. But not suited for sorting or db key. Use NewId if possible.</remarks>
    public class GuidKeyGenerator<T> : IKeyGenerator<T> where T : class
    {
        /// <inheritdoc />
        public KeyType KeyStrategy { get; } = KeyType.UniqId;

        /// <inheritdoc />
        public Key GetNextKey(IDocumentCollection<T> collection, T entity)
        {
            return new Key(KeyType.UniqId, Guid.NewGuid().ToString());
        }

        /// <inheritdoc />
        public Key GetNextKey(IDataStore dataStore, T entity)
        {
            return new Key(KeyType.UniqId, Guid.NewGuid().ToString());
        }
    }
}
