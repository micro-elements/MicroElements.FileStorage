using System;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.KeyGenerators
{
    public class SemanticKeyGenerator<T> : IKeyGenerator<T> where T : class
    {
        /// <inheritdoc />
        public KeyType KeyStrategy { get; } = KeyType.Semantic;

        /// <inheritdoc />
        public Key GetNextKey(IDocumentCollection<T> collection)
        {
            throw new NotSupportedException("GetNextKey is unsupported for Semantic keys.");
        }
    }
}