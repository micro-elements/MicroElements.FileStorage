// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.ZipEngine;
using Microsoft.Extensions.Logging;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace MicroElements.FileStorage.NuGetEngine
{
    public class NuGetStorageEngine : IStorageEngine, IDisposable
    {
        private ZipStorageEngine _zipStorageEngine;

        /// <inheritdoc />
        public NuGetStorageEngine(NuGetStorageConfiguration configuration, ILoggerFactory loggerFactory)
        {
            var packageSource = new PackageSource(configuration.PackageSource);
            var providers = new List<Lazy<INuGetResourceProvider>>(Repository.Provider.GetCoreV3());
            var sourceRepository = new SourceRepository(packageSource, providers);
            var downloadResource = sourceRepository.GetResource<DownloadResource>();
            var packageIdentity = new PackageIdentity(configuration.PackageId, NuGetVersion.Parse(configuration.PackageVersion));
            var sourceCacheContext = new SourceCacheContext { DirectDownload = configuration.DirectDownload };
            var packageDownloadContext = new PackageDownloadContext(sourceCacheContext, configuration.InstallPackagesFolder, configuration.DirectDownload);

            var nuGetLogger = new NuGetLogger(loggerFactory.CreateLogger<NuGetLogger>());
            var downloadResourceResult = downloadResource.GetDownloadResourceResultAsync(packageIdentity, packageDownloadContext, configuration.GlobalPackagesFolder, nuGetLogger, CancellationToken.None).Result;
            //var enumerable = downloadResourceResult.PackageReader.GetFiles().ToList();

            string packageFileName;
            if (!string.IsNullOrEmpty(configuration.InstallPackagesFolder))
            {
                Directory.CreateDirectory(configuration.InstallPackagesFolder);

                packageFileName = Path.Combine(configuration.InstallPackagesFolder, $"{packageIdentity.Id}.{packageIdentity.Version}.nupkg");
                if (!File.Exists(packageFileName))
                {
                    using (var fileStream = new FileStream(packageFileName, FileMode.OpenOrCreate))
                    {
                        downloadResourceResult.PackageStream.CopyTo(fileStream);
                    }
                }
            }
            else
            {
                packageFileName = "todo_get";
            }

            _zipStorageEngine = new ZipStorageEngine(new ZipStorageConfiguration(packageFileName)
            {
                StreamType = ZipStorageEngineStreamType.MemoryStream,
                LeaveOpen = false
            });
        }

        /// <inheritdoc />
        public Task<FileContent> ReadFile(string subPath)
        {
            return _zipStorageEngine.ReadFile(subPath);
        }

        /// <inheritdoc />
        public IEnumerable<Task<FileContent>> ReadDirectory(string subPath)
        {
            return _zipStorageEngine.ReadDirectory(subPath);
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

        /// <inheritdoc />
        public void Dispose()
        {
            _zipStorageEngine?.Dispose();
        }
    }
}
