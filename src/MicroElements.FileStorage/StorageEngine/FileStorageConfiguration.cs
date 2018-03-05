// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.FileStorage.StorageEngine
{
    /// <summary>
    /// Configuration for <see cref="FileStorageProvider"/>.
    /// </summary>
    public class FileStorageConfiguration : IFileStorageConfiguration
    {
        /// <inheritdoc />
        public string BasePath { get; set; }

        /// <inheritdoc />
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageConfiguration"/> class.
        /// </summary>
        public FileStorageConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageConfiguration"/> class.
        /// </summary>
        /// <param name="basePath">BasePath.</param>
        /// <param name="readOnly">Storage is ReadOnly</param>
        public FileStorageConfiguration(string basePath, bool readOnly = false)
        {
            BasePath = basePath;
            ReadOnly = readOnly;
        }
    }
}
