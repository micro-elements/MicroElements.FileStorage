// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.KeyGenerators
{
    /// <summary>
    /// KeyGenerator that generates key with user defined func.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class KeyGenerator<T> : IKeyGenerator<T> where T : class
    {
        private readonly Func<T, string> _getKeyfunc;

        public KeyGenerator(Func<T, string> getKeyFunc, KeyType keyStrategy)
        {
            _getKeyfunc = getKeyFunc;
            KeyStrategy = keyStrategy;
        }

        /// <inheritdoc />
        public KeyType KeyStrategy { get; }

        /// <inheritdoc />
        public Key GetNextKey(IDataStore dataStore, T entity)
        {
            var key = _getKeyfunc(entity);
            return new Key(KeyStrategy, key);
        }
    }
}
