// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MicroElements.FileStorage.StorageEngine;

namespace MicroElements.FileStorage.NuGetEngine
{
    /// <summary>
    /// Configuration for <see cref="NuGetStorageProvider"/>.
    /// </summary>
    public interface INuGetStorageConfiguration : IFileStorageConfiguration
    {
        /// <summary>
        /// Nuget package source.
        /// </summary>
        string PackageSource { get; }

        /// <summary>
        /// Package id.
        /// </summary>
        string PackageId { get; }

        /// <summary>
        /// Package version.
        /// </summary>
        string PackageVersion { get; }

        /// <summary>
        /// Global package cache folder. All packages cached in this folder.
        /// </summary>
        string GlobalPackagesFolder { get; }

        /// <summary>
        /// If direct download then <see cref="GlobalPackagesFolder"/> is not uses.
        /// </summary>
        bool DirectDownload { get; }

        /// <summary>
        /// Folder for install packages.
        /// </summary>
        string InstallPackagesFolder { get; }
    }
}
