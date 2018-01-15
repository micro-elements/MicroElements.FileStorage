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

namespace MicroElements.FileStorage.StorageEngine
{
    public class ZipStorageEngine : IStorageEngine, IDisposable
    {
        private readonly Stream _zipArchiveStream;
        private readonly ZipArchive _zipArchive;
        private readonly bool _leaveOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipStorageEngine"/> class.
        /// Create a ZipStorageEngine by path.
        /// </summary>
        /// <param name="path">A relative or absolute path for the archive.</param>
        /// <param name="zipStorageEngineTypeWork">To use fileStram or MemoryStream. For Memory available in read-only mode. For FileStream available in read-write mode.</param>
        /// <param name="leaveOpen">True to leave the stream open after the ZiptorageEngine object is disposed; otherwise, false</param>
        public ZipStorageEngine(string path, ZipStorageEngineStreamType zipStorageEngineTypeWork, bool leaveOpen = false)
        {
            var zipArchiveMode = ZipArchiveMode.Read;
            Stream zipStream = null;
            switch (zipStorageEngineTypeWork)
            {
                case ZipStorageEngineStreamType.FileStream:
                    zipArchiveMode = ZipArchiveMode.Update;
                    zipStream = new FileStream(path, FileMode.Open);
                    break;
                case ZipStorageEngineStreamType.MemoryStream:
                    zipStream = new MemoryStream(File.ReadAllBytes(path));
                    break;
                default:
                    throw new NotImplementedException();
            }

            _leaveOpen = leaveOpen;
            _zipArchiveStream = zipStream;
            _zipArchive = new ZipArchive(_zipArchiveStream, zipArchiveMode, leaveOpen);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipStorageEngine"/> class.
        /// </summary>
        /// <param name="stream">The input or output stream.</param>
        /// <param name="mode">Read-only or read-write mode.</param>
        /// <param name="leaveOpen">True to leave the stream open after the ZiptorageEngine object is disposed; otherwise, false</param>
        public ZipStorageEngine(Stream stream, ZipStorageEngineMode mode = ZipStorageEngineMode.Read, bool leaveOpen = false)
        {
            _leaveOpen = leaveOpen;
            ZipArchiveMode zipArchiveMode;
            switch (mode)
            {
                case ZipStorageEngineMode.Read:
                    zipArchiveMode = ZipArchiveMode.Read;
                    break;
                case ZipStorageEngineMode.Write:
                    zipArchiveMode = ZipArchiveMode.Update;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }

            _zipArchiveStream = stream;
            _zipArchive = new ZipArchive(_zipArchiveStream, zipArchiveMode, leaveOpen);
        }

        /// <inheritdoc/>
        public IEnumerable<Task<FileContent>> ReadDirectory([NotNull] string subPath)
        {
            var zipEntries = _zipArchive.Entries.Where(p => p.FullName.StartsWith(subPath));
            return zipEntries.Select(GetFileContentFromZipEntry);
        }

        /// <inheritdoc/>
        public async Task<FileContent> ReadFile([NotNull] string subPath)
        {
            var zipEntry = _zipArchive.GetEntry(subPath);
            return await GetFileContentFromZipEntry(zipEntry);
        }

        /// <inheritdoc/>
        public async Task WriteFile([NotNull] string subPath, [NotNull] FileContent content)
        {
            var zipEntry = _zipArchive.CreateEntry(subPath);

            using (var fileZipStream = zipEntry.Open())
            {
                using (var writeStream = new StreamWriter(fileZipStream))
                {
                    await writeStream.WriteAsync(content.Content);
                }
            }
        }

        public ZipArchive GetZipArchiveReadOnlyAndDispose()
        {
            _zipArchive.Dispose();
            _zipArchiveStream.Seek(0, SeekOrigin.Begin);
            var readOnlyArchive = new ZipArchive(_zipArchiveStream, ZipArchiveMode.Read);
            Dispose();
            return readOnlyArchive;
        }

        private static async Task<FileContent> GetFileContentFromZipEntry(ZipArchiveEntry zipEntry)
        {
            var content = string.Empty;
            var location = string.Empty;
            if (zipEntry != null)
            {
                var fileZipStream = zipEntry.Open();
                var writeStream = new StreamReader(fileZipStream);
                content = await writeStream.ReadToEndAsync();
                location = zipEntry.FullName;
            }

            return new FileContent(location, content);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!_leaveOpen)
                    {
                        _zipArchiveStream.Dispose();
                    }

                    _zipArchive.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
