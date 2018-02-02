// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.Abstractions.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MicroElements.FileStorage
{
    /// <summary>
    /// DataStore configuration.
    /// </summary>
    public class DataStoreConfiguration
    {
        /// <summary>
        /// StorageEngine.
        /// </summary>
        public IStorageEngine StorageEngine { get; set; }

        /// <summary>
        /// Collection definitions.
        /// </summary>
        public CollectionConfiguration[] Collections { get; set; }

        public ILoggerFactory LoggerFactory { get; set; } = NullLoggerFactory.Instance;

        public Conventions Conventions { get; set; } = Conventions.Default;

        public void Verify()
        {
            if (StorageEngine == null)
            {
                throw new InvalidConfigurationException("StorageEngine is required");
            }

            foreach (var configuration in Collections)
            {
                configuration.Verify();
            }
        }
    }
}
