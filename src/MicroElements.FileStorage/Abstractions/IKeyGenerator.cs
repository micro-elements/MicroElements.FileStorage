namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Key generator.
    /// <para>Uses for key generation for new entities.</para>
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public interface IKeyGenerator<T> where T : class
    {
        /// <summary>
        /// Key strategy.
        /// </summary>
        KeyType KeyStrategy { get; }

        /// <summary>
        /// Generates new key for collection.
        /// </summary>
        /// <param name="collection">Document collection.</param>
        /// <param name="entity">Entity.</param>
        /// <returns>New key.</returns>
        Key GetNextKey(IDocumentCollection<T> collection, T entity);
    }
}