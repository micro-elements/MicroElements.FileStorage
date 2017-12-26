using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Data reader reads file content.
    /// </summary>
    public interface IDataReader
    {
        /// <summary>
        /// Reads file content.
        /// </summary>
        /// <param name="subPath">Path relative to root path of DataReader.</param>
        /// <returns><see cref="FileContent"/></returns>
        Task<FileContent> ReadFile([NotNull] string subPath);

        /// <summary>
        /// Reads directory.
        /// </summary>
        /// <param name="subPath">Path relative to root path of DataReader.</param>
        /// <returns>ReadFile tasks.</returns>
        IEnumerable<Task<FileContent>> ReadDirectory([NotNull] string subPath);
    }
}