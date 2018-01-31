// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;
using Microsoft.Extensions.FileProviders;

namespace MicroElements.FileStorage.StorageEngine
{
    public class FileProviderStorageEngine : IStorageEngine
    {
        private readonly IFileProvider _fileProvider;

        public FileProviderStorageEngine(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public async Task<FileContent> ReadFile(string subPath)
        {
            var fileInfo = _fileProvider.GetFileInfo(subPath);
            if (fileInfo.Exists)
            {
                using (var stream = fileInfo.CreateReadStream())
                using (var streamReader = new StreamReader(stream))
                {
                    string content = await streamReader.ReadToEndAsync();
                    return new FileContent(subPath, content);
                }
            }
            //todo: log
            return new FileContent(subPath, string.Empty);
        }

        public IEnumerable<Task<FileContent>> ReadDirectory(string subPath)
        {
            var directoryContents = _fileProvider.GetDirectoryContents(subPath);
            foreach (var fileInfo in directoryContents)
            {
                yield return ReadFile(Path.Combine(subPath, fileInfo.Name));
            }
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
            return new StorageMetadata
            {
                IsReadonly = true
            };
        }
    }
}
