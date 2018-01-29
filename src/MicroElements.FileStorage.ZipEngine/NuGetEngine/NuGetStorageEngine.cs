// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.Abstractions.Exceptions;
using MicroElements.FileStorage.CodeContracts;
using MicroElements.FileStorage.ZipEngine;
using Microsoft.Extensions.Logging;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace MicroElements.FileStorage.NuGetEngine
{
    /// <summary>
    /// NuGet storage engine.
    /// <para>Downloads nuget package and uses as data source.</para>
    /// </summary>
    public class NuGetStorageEngine : IStorageEngine, IDisposable
    {
        private readonly ZipStorageEngine _zipStorageEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetStorageEngine"/> class.
        /// </summary>
        /// <param name="configuration"><see cref="NuGetStorageConfiguration"/>.</param>
        /// <param name="loggerFactory"><see cref="ILoggerFactory"/>.</param>
        public NuGetStorageEngine([NotNull] NuGetStorageConfiguration configuration, [NotNull] ILoggerFactory loggerFactory)
        {
            Check.NotNull(configuration, nameof(configuration));
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            var packageSource = new PackageSource(configuration.PackageSource);
            var providers = new List<Lazy<INuGetResourceProvider>>(Repository.Provider.GetCoreV3());
            var sourceRepository = new SourceRepository(packageSource, providers);
            var downloadResource = sourceRepository.GetResource<DownloadResource>();
            var packageIdentity = new PackageIdentity(configuration.PackageId, NuGetVersion.Parse(configuration.PackageVersion));
            var sourceCacheContext = new SourceCacheContext { DirectDownload = configuration.DirectDownload };
            var packageDownloadContext = new PackageDownloadContext(sourceCacheContext, configuration.InstallPackagesFolder, configuration.DirectDownload);

            var logger = new NuGetLogger(loggerFactory.CreateLogger<NuGetLogger>());
            var downloadResourceResult = downloadResource.GetDownloadResourceResultAsync(packageIdentity, packageDownloadContext, configuration.GlobalPackagesFolder, logger, CancellationToken.None).Result;
            //var enumerable = downloadResourceResult.PackageReader.GetFiles().ToList();

            string packageFileName;
            if (!string.IsNullOrEmpty(configuration.InstallPackagesFolder))
            {
                Directory.CreateDirectory(configuration.InstallPackagesFolder);

                packageFileName = Path.Combine(configuration.InstallPackagesFolder, $"{packageIdentity.Id}.{packageIdentity.Version}.nupkg");
                if (!File.Exists(packageFileName))
                {
                    if (downloadResourceResult.PackageStream == null)
                    {
                        throw new FileStorageException($"Package is not found. Status: {downloadResourceResult.Status}");
                    }

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
                Mode = ZipStorageEngineMode.Read,
                LeaveOpen = false,
                BasePath = configuration.BasePath
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
