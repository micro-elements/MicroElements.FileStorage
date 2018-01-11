// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.CodeContracts;
using MicroElements.FileStorage.Utils;

namespace MicroElements.FileStorage.StorageEngine
{
    /// <summary>
    /// FileStorageEngine.
    /// <para>Data stores in file system.</para> 
    /// </summary>
    public class FileStorageEngine : IStorageEngine
    {
        private readonly string _basePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageEngine"/> class.
        /// </summary>
        /// <param name="basePath">Base path.</param>
        public FileStorageEngine([NotNull] string basePath)
        {
            Check.NotNull(basePath, nameof(basePath));

            _basePath = basePath;
            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);
        }

        /// <inheritdoc />
        public async Task<FileContent> ReadFile(string subPath)
        {
            Check.NotNull(subPath, nameof(subPath));

            var location = Path.Combine(_basePath, subPath);
            string text = string.Empty;
            if (File.Exists(location))
            {
                text = await FileAsync.ReadAllText(location);
            }
            return new FileContent(location, text);
        }

        /// <inheritdoc />
        public IEnumerable<Task<FileContent>> ReadDirectory(string subPath)
        {
            Check.NotNull(subPath, nameof(subPath));

            var location = Path.Combine(_basePath, subPath);
            if (Directory.Exists(location))
            {
                foreach (var file in Directory.EnumerateFiles(location))
                {
                    yield return ReadFile(file);
                }
            }
        }

        /// <inheritdoc />
        public async Task WriteFile(string subPath, FileContent content)
        {
            Check.NotNull(content, nameof(content));
            Check.NotNull(subPath, nameof(subPath));

            var location = Path.Combine(_basePath, subPath);
            var directoryName = Path.GetDirectoryName(location);
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            await FileAsync.WriteAllText(location, content.Content);
        }
    }
}
