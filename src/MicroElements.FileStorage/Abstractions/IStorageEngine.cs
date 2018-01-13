// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Storage for underlying storage system.
    /// </summary>
    public interface IStorageEngine
    {
        /// <summary>
        /// Reads file content.
        /// </summary>
        /// <param name="subPath">Path relative to the root path of StorageEngine.</param>
        /// <returns><see cref="FileContent"/></returns>
        Task<FileContent> ReadFile([NotNull] string subPath);

        /// <summary>
        /// Reads directory.
        /// </summary>
        /// <param name="subPath">Path relative to the root path of StorageEngine.</param>
        /// <returns>ReadFile tasks.</returns>
        IEnumerable<Task<FileContent>> ReadDirectory([NotNull] string subPath);

        /// <summary>
        /// Writes file to destination.
        /// </summary>
        /// <param name="subPath">Path relative to the root path of StorageEngine.</param>
        /// <param name="content">Content to write.</param>
        /// <returns>WriteFile task.</returns>
        Task WriteFile([NotNull] string subPath, [NotNull] FileContent content);

        /// <summary>
        /// Deletes file.
        /// </summary>
        /// <param name="subPath">Path relative to the root path of StorageEngine.</param>
        /// <returns>Delete task.</returns>
        Task DeleteFile([NotNull] string subPath);

        /// <summary>
        /// Gets file information (metadata).
        /// </summary>
        /// <param name="subPath">Path relative to the root path of StorageEngine.</param>
        /// <returns><see cref="FileContentMetadata"/></returns>
        [NotNull] FileContentMetadata GetFileMetadata([NotNull] string subPath);

        /// <summary>
        /// Gets storage information (metadata).
        /// </summary>
        /// <returns><see cref="StorageMetadata"/></returns>
        [NotNull] StorageMetadata GetStorageMetadata();
    }
}
