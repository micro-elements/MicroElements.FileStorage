// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.FileStorage.StorageEngine
{
    /// <summary>
    /// Common properties for storage engines.
    /// </summary>
    public interface IFileStorageConfiguration
    {
        /// <summary>
        /// Name of Storage.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Base path to search files.
        /// </summary>
        string BasePath { get; }

        /// <summary>
        /// Gets a value indicating whether storage is writable.
        /// <para>Any change operations are prohibited for read only storages.</para>
        /// </summary>
        bool ReadOnly { get; }
    }
}
