using System;
using System.Linq.Expressions;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.KeyGenerators
{
    public class SemanticKeyGenerator<T> : IKeyGenerator<T> where T : class
    {
        private Expression<Func<T, string>> _getKeyExpression;
        private Func<T, string> _getKeyfunc;

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
    }
}