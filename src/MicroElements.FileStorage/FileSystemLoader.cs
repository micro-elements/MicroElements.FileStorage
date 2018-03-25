// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;
using Microsoft.Extensions.Logging;

namespace MicroElements.FileStorage
{
    public class FileSystemLoader
    {
        private LoaderSettings _loaderSettings;
        private readonly IDataStore _dataStore;
        private ILogger _logger;

        public FileSystemLoader(LoaderSettings loaderSettings)
        {
            _loaderSettings = loaderSettings;
            _dataStore = loaderSettings.DataStore;
            _logger = loaderSettings.DataStore.Services.LoggerFactory.CreateLogger(typeof(FileSystemLoader));
        }

        public async Task<IDictionary<Type, CollectionData>> LoadEntitiesAsync()
        {
            ConcurrentDictionary<Type, CollectionData> entityLists = new ConcurrentDictionary<Type, CollectionData>();

            _logger.LogDebug("Collection loading started");

            foreach (var documentType in _dataStore.Schema.DocumentTypes)
            {
                var loadEntityListMethod = GetType().GetMethod(nameof(LoadEntityList), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(documentType);
                var loadEntityListTask = (Task<CollectionData>)loadEntityListMethod.Invoke(this, null);
                var entityList = await loadEntityListTask;

                entityLists.TryAdd(documentType, entityList);
                _logger.LogInformation($"Collection loaded. Type: {documentType}");
            }

            return entityLists;
        }

        private async Task<CollectionData> LoadEntityList<T>() where T : class
        {
            var storageConfig = _loaderSettings.DataStorageConfiguration;
            var documentType = typeof(T);

            var collectionConfig = storageConfig.Collections
                .FirstOrDefault(config => config.DocumentType == documentType);

            if (collectionConfig == null)
            {
                return new CollectionData();
            }

            var serializer = GetSerializer(collectionConfig);

            var fileTasks = GetFileTasks(collectionConfig, storageConfig);
            var fileContents = await ReadAllInOneThread(fileTasks);
            var deserialized = DeserializeEntitiesInOneThread<T>(fileContents, serializer);

            T[] entities = deserialized.ToArray();
            string[] keys = new string[entities.Length];

            for (var index = 0; index < entities.Length; index++)
            {
                var entity = entities[index];
                var configurationTyped = collectionConfig.ToTyped<T>();
                keys[index] = configurationTyped.KeyGetter.GetIdFunc()(entity);
            }

            return new CollectionData()
            {
                EntityType = typeof(T),
                Entities = entities,
                Keys = keys,
                DeletedKeys = null
            };
        }

        private List<T> DeserializeEntitiesInOneThread<T>(FileContent[] fileContents, ISerializer serializer)
        {
            var entities = new List<T>();
            foreach (var fileContent in fileContents)
            {
                var objects = serializer.Deserialize<T>(fileContent);
                entities.AddRange(objects);
            }

            return entities;
        }

        private IEnumerable<Task<FileContent>> GetFileTasks(ICollectionConfiguration collectionConfig, IDataStorageConfiguration storageConfig)
        {
            var storageProvider = storageConfig.StorageProvider;

            IEnumerable<Task<FileContent>> fileTasks;

            if (IsMultiFile(collectionConfig))
            {
                fileTasks = storageProvider.ReadDirectory(collectionConfig.SourceFile);
            }
            else
            {
                fileTasks = new[] { storageProvider.ReadFile(collectionConfig.SourceFile) };
            }

            return fileTasks;
        }

        private async Task<FileContent[]> ReadAllInOneThread(IEnumerable<Task<FileContent>> fileTasks)
        {
            var fileContents = new ConcurrentDictionary<string, FileContent>();
            foreach (var fileTask in fileTasks)
            {
                var fileContent = await fileTask;
                fileContents.TryAdd(fileContent.Location, fileContent);
            }

            return fileContents.Values.ToArray();
        }

        public IEntityList CreateEntityList<T>(
            bool readOnly,
            IList<T> entities,
            CollectionConfiguration<T> collectionConfig)
            where T : class
        {
            string GetKey(T item) => collectionConfig.KeyGetter.GetIdFunc()(item);

            var entityWithKeys = entities.Select(ent => new EntityWithKey<T>(ent, GetKey(ent))).ToList();
            if (readOnly)
            {
                return new ReadOnlyEntityList<T>(new ExportData<T>
                {
                    Added = entityWithKeys
                });
            }
            else
            {
                var entityList = new WritableEntityList<T>();

                foreach (var entityWithKey in entityWithKeys)
                {
                    entityList.AddOrUpdate(entityWithKey.Entity, entityWithKey.Key);
                }

                return entityList;
            }
        }

        public ISerializer GetSerializer(ICollectionConfiguration configuration)
        {
            return configuration.Serializer ?? _dataStore.Configuration.Conventions.GetSerializer(configuration);
        }

        public bool IsMultiFile(ICollectionConfiguration configuration)
        {
            return configuration.IsMultiFile();
        }
    }

    public class LoaderSettings
    {
        public IDataStore DataStore { get; }

        public IDataStorageConfiguration DataStorageConfiguration { get; }

        public LoaderSettings(IDataStore dataStore, IDataStorageConfiguration dataStorageConfiguration)
        {
            DataStore = dataStore;
            DataStorageConfiguration = dataStorageConfiguration;
        }
    }
}
