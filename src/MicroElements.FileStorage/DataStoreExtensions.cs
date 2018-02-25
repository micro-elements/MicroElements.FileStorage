// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.CodeContracts;
using MicroElements.FileStorage.Operations;

namespace MicroElements.FileStorage
{
    /// <summary>
    /// Extensions for <see cref="IDataStore"/>.
    /// </summary>
    public static class DataStoreExtensions
    {
        /// <summary>
        /// Gets write provider.
        /// </summary>
        /// <param name="dataStore">The DataStore.</param>
        /// <returns><see cref="IStorageProvider"/> for write.</returns>
        /// <exception cref="InvalidOperationException">Last storage provider in DataStore is ReadOnly.</exception>
        public static IStorageProvider GetWriteProvider([NotNull] this IDataStore dataStore)
        {
            Check.NotNull(dataStore, nameof(dataStore));

            var storageProvider = dataStore.Configuration.Storages.Last().StorageProvider;
            if (storageProvider.GetStorageMetadata().IsReadOnly)
            {
                throw new InvalidOperationException("Last storage provider in DataStore is ReadOnly!");
            }

            return storageProvider;
        }

        public static string GetKey<T>(this IDocumentCollection<T> collection, T item) where T : class
        {
            return collection.ConfigurationTyped.KeyGetter.GetIdFunc()(item);
        }

        public static IEntityList<T> ToReadOnly<T>(this IEntityList<T> entityList) where T : class
        {
            if (entityList is IExportable<T> canExport)
            {
                var exportData = canExport.Export();
                return new ReadOnlyEntityList<T>(exportData);
            }
            throw new Exception("Enumeration needed");
        }

        public static CollectionConfigurationTyped<T> ToTyped<T>(this CollectionConfiguration configuration) where T : class
        {
            if (configuration is CollectionConfigurationTyped<T> typed)
                return typed;

            if (configuration.DocumentType == typeof(T))
                return new CollectionConfigurationTyped<T>(configuration);

            return new CollectionConfigurationTyped<T>();
        }

        public static CollectionConfigurationTyped<T> GetConfigurationTyped<T>(this IDataStore dataStore) where T : class
        {
            var collectionConfiguration = dataStore.Configuration.Storages.Last().Collections
                .FirstOrDefault(configuration => configuration.DocumentType == typeof(T))
                .ToTyped<T>();
            return collectionConfiguration;
        }

        public static bool IsReadOnly(this DataStoreConfiguration dataStoreConfiguration)
        {
            return dataStoreConfiguration.ReadOnly || dataStoreConfiguration.Storages.All(configuration => configuration.IsReadOnly());
        }

        public static bool IsReadOnly(this IDataStorage dataStorage)
        {
            return dataStorage.Configuration.IsReadOnly();
        }

        public static bool IsReadOnly(this IDataStorageConfiguration dataStorageConfiguration)
        {
            return dataStorageConfiguration.ReadOnly || dataStorageConfiguration.StorageProvider.GetStorageMetadata().IsReadOnly;
        }

        public static bool IsWritable(this IDataStorage dataStorage)
        {
            return !dataStorage.IsReadOnly();
        }

        [CanBeNull]
        public static IWritableDataStorage GetWritableStorage([NotNull] this IDataStore dataStore)
        {
            Check.NotNull(dataStore, nameof(dataStore));

            return dataStore.Storages.FirstOrDefault(storage => storage.IsWritable()) as IWritableDataStorage;
        }

        [Obsolete("Used for debugging. Remove ToArray for performance reason.")]
        public static T[] ToArrayTemp<T>(this IEnumerable<T> items)
        {
            return items.ToArray();
        }

        [Obsolete("Used for debugging. Remove ToList for performance reason.")]
        public static List<T> ToListTemp<T>(this IEnumerable<T> items)
        {
            return items.ToList();
        }

        public static ISerializer GetSerializer(this DataStoreConfiguration dataStoreConfiguration, Type entityType)
        {
            CollectionConfiguration collectionConfiguration = dataStoreConfiguration.GetCollectionConfiguration(entityType);

            if (collectionConfiguration?.Serializer != null)
                return collectionConfiguration.Serializer;

            return dataStoreConfiguration.Conventions.GetSerializer(collectionConfiguration);
        }

        public static CollectionConfiguration GetCollectionConfiguration(this DataStoreConfiguration dataStoreConfiguration, Type entityType)
        {
            CollectionConfiguration collectionConfiguration = null;
            foreach (var storageConfiguration in dataStoreConfiguration.Storages)
            {
                collectionConfiguration = storageConfiguration.Collections.FirstOrDefault(configuration =>
                    configuration.DocumentType == entityType);
                if (collectionConfiguration != null)
                    break;
            }
            return collectionConfiguration;
        }

        public static bool IsMultiFile(this CollectionConfiguration configuration)
        {
            var isDirectory = !Path.HasExtension(configuration.SourceFile);
            return isDirectory;
        }
    }
}
