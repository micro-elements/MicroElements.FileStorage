// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace MicroElements.FileStorage.NuGetEngine
{
    public class NuGetStorageEngine : IStorageEngine
    {
        /// <inheritdoc />
        public NuGetStorageEngine()
        {
            var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");

            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());  // Add v3 API support
            //providers.AddRange(Repository.Provider.GetCoreV2());  // Add v2 API support
            //providers.Add(new Lazy<INuGetResourceProvider>(() => new DownloadResourceV3Provider()));
            var sourceRepository = new SourceRepository(packageSource, providers);
            var downloadResourceV3 = sourceRepository.GetResource<DownloadResource>();
            var packageIdentity = new PackageIdentity("NUnit", NuGetVersion.Parse("3.9.0"));
            var sourceCacheContext = new SourceCacheContext();
            var packageDownloadContext = new PackageDownloadContext(sourceCacheContext);
            var downloadResourceResult = downloadResourceV3.GetDownloadResourceResultAsync(packageIdentity,
                packageDownloadContext, "globalPackagesFolder", new NullLogger(), CancellationToken.None).Result;
            var enumerable = downloadResourceResult.PackageReader.GetFiles().ToList();

        }

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
