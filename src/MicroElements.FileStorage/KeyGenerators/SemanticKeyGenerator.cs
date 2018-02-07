// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.KeyGenerators
{
    /// <summary>
    /// Key generator based on entity values.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class SemanticKeyGenerator<T> : IKeyGenerator<T> where T : class
    {
        private readonly Expression<Func<T, string>> _getKeyExpression;
        private readonly Func<T, string> _getKeyfunc;

        public SemanticKeyGenerator(Expression<Func<T, string>> getKeyExpression)
        {
            _getKeyExpression = getKeyExpression;
            _getKeyfunc = _getKeyExpression.Compile();
        }

        /// <inheritdoc />
        public KeyType KeyStrategy { get; } = KeyType.Semantic;

        /// <inheritdoc />
        public Key GetNextKey(IDocumentCollection<T> collection, T entity)
        {
            var key = _getKeyfunc(entity);
            return new Key(KeyType.Semantic, key);
        }

        /// <inheritdoc />
        public Key GetNextKey(IDataStore dataStore, T entity)
        {
            var key = _getKeyfunc(entity);
            return new Key(KeyType.Semantic, key);
        }
    }
}
