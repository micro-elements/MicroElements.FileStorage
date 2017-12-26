using System;
using System.Threading.Tasks;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// DataStore is document database.
    /// </summary>
    public interface IDataStore : IDisposable
    {
        /// <summary>
        /// Loads data.
        /// </summary>
        Task Initialize();

        /// <summary>
        /// Get collection.
        /// </summary>
        /// <typeparam name="T">Document type.</typeparam>
        /// <returns>Typed IDocumentCollection.</returns>
        IDocumentCollection<T> GetCollection<T>() where T : class;
    }
}