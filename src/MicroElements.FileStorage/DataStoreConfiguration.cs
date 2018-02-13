// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using FluentValidation;
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
        /// StorageProvider.
        /// </summary>
        [Obsolete("Use Storages")]
        public IStorageProvider StorageProvider
        {
            get => Storages.First().StorageProvider;
            set => Storages.First().StorageProvider = value;
        }

        /// <summary>
        /// Collection definitions.
        /// </summary>
        [Obsolete("Use Storages")]
        public CollectionConfiguration[] Collections
        {
            get => Storages.First().Collections;
            set => Storages.First().Collections = value;
        }

        public bool ReadOnly { get; set; } = false;

        public DataStorageConfiguration[] Storages { get; set; } = { new DataStorageConfiguration() };

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

    public class DataStorageConfigurationValidator : AbstractValidator<DataStorageConfiguration>
    {
        public DataStorageConfigurationValidator()
        {
            RuleFor(configuration => configuration.StorageProvider).NotNull();
        }
    }
}
