// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.FileStorage.StorageEngine
{
    /// <summary>
    /// Common properties for storage engines.
    /// </summary>
    public class CommonStorageConfiguration
    {
        /// <summary>
        /// Base path to search files.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// Gets a value indicating whether storage is writable.
        /// <para>Any change operation prohibited for read only storages.</para>
        /// </summary>
        public bool IsReadOnly { get; set; } = false;
    }

    public interface IStorageConfiguration
    {
        string Name { get; }
        int Order { get; }
        bool IsActive { get; }
        string[] Types { get; }
        bool IsDefault { get; }
    }
}
