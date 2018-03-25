// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.Abstractions.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MicroElements.FileStorage
{
    /// <summary>
    /// DataStore configuration.
    /// </summary>
    public class DataStoreConfiguration : IDataStoreConfiguration
    {
        public bool ReadOnly { get; set; } = false;

        public IReadOnlyList<IDataStorageConfiguration> Storages { get; set; } = Array.Empty<IDataStorageConfiguration>();

        public ILoggerFactory LoggerFactory { get; set; } = NullLoggerFactory.Instance;

        public Conventions Conventions { get; set; } = Conventions.Default;

        public void Verify()
        {
            if (Storages == null)
            {
                throw new InvalidConfigurationException("Storages is required");
            }

            foreach (var storageConfiguration in Storages)
            {
                storageConfiguration.Verify();
            }
        }
    }
}
