// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.CodeContracts;

namespace MicroElements.FileStorage.ZipEngine
{
    /// <summary>
    /// Zip storage engine.
    /// </summary>
    public sealed class ZipStorageProvider : IStorageProvider, IDisposable
    {
        private readonly ZipStorageConfiguration _configuration;
        private readonly Stream _zipArchiveStream;
        private readonly ZipArchive _zipArchive;
        private readonly string _basePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipStorageProvider"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public ZipStorageProvider([NotNull] ZipStorageConfiguration configuration)
        {
            Check.NotNull(configuration, nameof(configuration));
            _configuration = configuration;

            if (!string.IsNullOrEmpty(configuration.BasePath))
            {
                _basePath = NormalizeSlashes(configuration.BasePath);
                if (!_basePath.EndsWith("/"))
                    _basePath += "/";
            }

            if (configuration.Stream != null)
            {
                _zipArchiveStream = configuration.Stream;
            }
            else
            {
                switch (configuration.StreamType)
                {
                    case ZipStorageEngineStreamType.FileStream:
                        _zipArchiveStream = new FileStream(configuration.Path, FileMode.Open);
                        break;
                    case ZipStorageEngineStreamType.MemoryStream:
                        _zipArchiveStream = new MemoryStream(File.ReadAllBytes(configuration.Path));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            ZipArchiveMode zipArchiveMode;
            switch (configuration.Mode)
            {
                case ZipStorageEngineMode.Read:
                    zipArchiveMode = ZipArchiveMode.Read;
                    break;
                case ZipStorageEngineMode.Write:
                    zipArchiveMode = ZipArchiveMode.Update;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(configuration.Mode));
            }

            _zipArchive = new ZipArchive(_zipArchiveStream, zipArchiveMode, configuration.LeaveOpen);
        }

        /// <inheritdoc/>
        public IEnumerable<Task<FileContent>> ReadDirectory(string subPath)
        {
            subPath = PreparePath(subPath);
            var zipEntries = _zipArchive.Entries.Where(p => p.FullName.StartsWith(subPath));
            return zipEntries.Select(GetFileContentFromZipEntry);
        }

        /// <inheritdoc/>
        public async Task<FileContent> ReadFile(string subPath)
        {
            var zipEntry = _zipArchive.GetEntry(PreparePath(subPath));
            return await GetFileContentFromZipEntry(zipEntry);
        }

        /// <inheritdoc/>
        public async Task WriteFile(string subPath, FileContent content)
        {
            var zipEntry = _zipArchive.CreateEntry(PreparePath(subPath));

            using (var fileZipStream = zipEntry.Open())
            {
                using (var writeStream = new StreamWriter(fileZipStream))
                {
                    await writeStream.WriteAsync(content.Content);
                }
            }
        }

        public Task DeleteFile(string subPath)
        {
            subPath = PreparePath(subPath);
            var zipEntry = _zipArchive.Entries.Single(p => p.FullName == subPath);
            zipEntry.Delete();
            return Task.CompletedTask;
        }

        public FileContentMetadata GetFileMetadata(string subPath)
        {
            throw new NotImplementedException();
        }

        public StorageMetadata GetStorageMetadata()
        {
            return new StorageMetadata
            {
                IsReadonly = _configuration.Mode != ZipStorageEngineMode.Write
            };
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _zipArchiveStream?.Dispose();
            _zipArchive?.Dispose();
        }

        public ZipArchive GetZipArchive()
        {
            return _zipArchive;
        }

        private string PreparePath(string path)
        {
            return _basePath != null ? _basePath + NormalizeSlashes(path) : NormalizeSlashes(path);
        }

        private string NormalizeSlashes(string path)
        {
            return path.Replace('\\', '/');
        }

        private static async Task<FileContent> GetFileContentFromZipEntry(ZipArchiveEntry zipEntry)
        {
            var content = string.Empty;
            var location = string.Empty;
            if (zipEntry != null)
            {
                using (var fileZipStream = zipEntry.Open())
                {
                    using (var writeStream = new StreamReader(fileZipStream))
                    {
                        content = await writeStream.ReadToEndAsync();
                    }
                }

                location = zipEntry.FullName;
            }

            return new FileContent(location, content);
        }
    }
}
