using System;
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
        /// Creates FileStorageEngine.
        /// </summary>
        /// <param name="basePath">Base path.</param>
        public FileStorageEngine([NotNull] string basePath)
        {
            Check.NotNull(basePath, nameof(basePath));

            _basePath = basePath;
        }

        /// <inheritdoc />
        public async Task<FileContent> ReadFile(string subPath)
        {
            Check.NotNull(subPath, nameof(subPath));

            var location = Path.Combine(_basePath, subPath);
            string text = String.Empty;
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
                foreach (var file in Directory.EnumerateFiles(location))
                {
                    yield return ReadFile(file);
                }
        }

        /// <inheritdoc />
        public async Task WriteFile(string subPath, FileContent content)
        {
            Check.NotNull(content, nameof(content));
            Check.NotNull(subPath, nameof(subPath));

            var location = Path.Combine(_basePath, subPath);
            await FileAsync.WriteAllText(location, content.Content);
        }
    }
}