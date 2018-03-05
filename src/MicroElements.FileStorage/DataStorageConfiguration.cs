// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage
{
    /// <summary>
    /// Configuration of DataStorage.
    /// </summary>
    public class DataStorageConfiguration : IDataStorageConfiguration
    {
        /// <summary>
        /// Parent storage.
        /// </summary>
        public DataStorageConfiguration Parent { get; set; }

        /// <summary>
        /// StorageProvider.
        /// </summary>
        public IStorageProvider StorageProvider { get; set; }

        /// <summary>
        /// Collection definitions.
        /// </summary>
        public ICollectionConfiguration[] Collections { get; set; }

        /// <summary>
        /// Gets a value indicating whether a storage is readonly.
        /// </summary>
        public bool ReadOnly { get; set; } = true;

        /// <summary>
        /// Verifies correctness of configuration.
        /// </summary>
        public void Verify()
        {
            new DataStorageConfigurationValidator().ValidateAndThrow(this);

            foreach (var configuration in Collections)
            {
                configuration.Verify();
            }
        }
    }
}
