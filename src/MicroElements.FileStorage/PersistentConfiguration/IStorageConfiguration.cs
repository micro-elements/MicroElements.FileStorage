// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using JetBrains.Annotations;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.StorageEngine;

namespace MicroElements.FileStorage.PersistentConfiguration
{
    public interface IDataStoreConfiguration
    {
        StorageConfiguration[] Storages { get; }
    }

    //todo: persistent data store configuration
    public interface IStorageConfiguration
    {
        string ProviderName { get; }
        string Name { get; }
        int Order { get; }
        bool IsActive { get; }
        string[] Types { get; }
        bool IsDefault { get; }
    }

    public class DataStoreConfiguration : IDataStoreConfiguration
    {
        /// <inheritdoc />
        public StorageConfiguration[] Storages { get; set; }
    }

    public class StorageConfiguration : IStorageConfiguration
    {
        /// <inheritdoc />
        public IFileStorageConfiguration Provider { get; set; }

        /// <inheritdoc />
        public string ProviderName { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public int Order { get; set; }

        /// <inheritdoc />
        public bool IsActive { get; set; }

        /// <inheritdoc />
        public string[] Types { get; set; }

        /// <inheritdoc />
        public bool IsDefault { get; set; }
    }

    /// <summary>
    /// DataStore registry.
    /// </summary>
    public interface IDataStoreRegistry
    {
        /// <summary>
        /// Gets <see cref="IDataStore"/> for type.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <returns><see cref="IDataStore"/> or null if not found.</returns>
        [CanBeNull] IDataStore GetDataStore([NotNull] Type entityType);
    }

    public interface ICollectionRegistry
    {
        [CanBeNull] ICollectionConfiguration GetCollection([NotNull] Type entityType);
    }
}
