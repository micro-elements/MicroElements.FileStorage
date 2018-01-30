// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.StorageEngine
{
    /// <summary>
    /// In memory storage engine. For test scenarios.
    /// </summary>
    public class InMemoryStorageEngine : IStorageEngine
    {
        private readonly Dictionary<string, FileContent> _contents = new Dictionary<string, FileContent>();

        /// <inheritdoc />
        public Task<FileContent> ReadFile(string subPath)
        {
            return Task.FromResult(_contents[subPath]);
        }

        /// <inheritdoc />
        public IEnumerable<Task<FileContent>> ReadDirectory(string subPath)
        {
            var keys = _contents.Keys.Where(path => path.Contains(subPath));
            foreach (var key in keys)
            {
                yield return Task.FromResult(_contents[key]);
            }
        }

        /// <inheritdoc />
        public Task WriteFile(string subPath, FileContent content)
        {
            _contents[subPath] = content;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DeleteFile(string subPath)
        {
            _contents.Remove(subPath);
            return Task.CompletedTask;
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
