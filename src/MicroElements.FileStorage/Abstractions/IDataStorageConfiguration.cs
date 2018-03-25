// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Configuration of DataStorage.
    /// </summary>
    public interface IDataStorageConfiguration
    {
        /// <summary>
        /// The StorageProvider.
        /// </summary>
        IStorageProvider StorageProvider { get; }

        /// <summary>
        /// Collection definitions.
        /// </summary>
        ICollectionConfiguration[] Collections { get; }

        /// <summary>
        /// Gets a value indicating whether a storage is readonly.
        /// </summary>
        bool ReadOnly { get; }

        /// <summary>
        /// Verifies correctness of configuration.
        /// </summary>
        void Verify();
    }
}
