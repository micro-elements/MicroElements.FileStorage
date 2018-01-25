// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.NuGetEngine
{
    public class NuGetStorageEngine : IStorageEngine
    {
        /// <inheritdoc />
        public Task<FileContent> ReadFile(string subPath)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IEnumerable<Task<FileContent>> ReadDirectory(string subPath)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task WriteFile(string subPath, FileContent content)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task DeleteFile(string subPath)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public FileContentMetadata GetFileMetadata(string subPath)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public StorageMetadata GetStorageMetadata()
        {
            throw new System.NotImplementedException();
        }
    }
}
