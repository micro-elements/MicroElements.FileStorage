// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        public IStorageProvider StorageProvider { get; set; }

        /// <summary>
        /// Collection definitions.
        /// </summary>
        public CollectionConfiguration[] Collections { get; set; }

        public DataStorageConfiguration[] Storages { get; set; }

        public ILoggerFactory LoggerFactory { get; set; } = NullLoggerFactory.Instance;

        public Conventions Conventions { get; set; } = Conventions.Default;

        public void Verify()
        {
            if (StorageProvider == null)
            {
                throw new InvalidConfigurationException("StorageProvider is required");
            }

            foreach (var configuration in Collections)
            {
                configuration.Verify();
            }
        }
    }

    public class DataStorageConfiguration
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
        public CollectionConfiguration[] Collections { get; set; }

        public void Verify()
        {
            new DataStorageConfigurationValidator().ValidateAndThrow(this);

            foreach (var configuration in Collections)
            {
                configuration.Verify();
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
