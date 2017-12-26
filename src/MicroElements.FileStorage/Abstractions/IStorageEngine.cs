using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Storage for underlying file system.
    /// </summary>
    public interface IStorageEngine
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

        /// <summary>
        /// Writes file to destination.
        /// </summary>
        /// <param name="subPath">Path relative to root path of DataReader.</param>
        /// <param name="content">Content to write.</param>
        /// <returns>WriteFile task.</returns>
        Task WriteFile([NotNull] string subPath, [NotNull] FileContent content);
    }
}