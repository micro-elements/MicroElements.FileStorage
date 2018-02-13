// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.Operations;
using Microsoft.Extensions.Logging;

namespace MicroElements.FileStorage
{
    public class DataLoader
    {
        private readonly IDataStore _dataStore;
        private Conventions _conventions;
        private ILogger _logger;

        private readonly DataStorageConfiguration _dataStorageConfiguration;

        /// <inheritdoc />
        public DataLoader(IDataStore dataStore, DataStorageConfiguration dataStorageConfiguration)
        {
            _dataStore = dataStore;
            _dataStorageConfiguration = dataStorageConfiguration;
            _conventions = dataStore.Configuration.Conventions;
            _logger = dataStore.Services.LoggerFactory.CreateLogger(typeof(DataLoader));
        }

        public async Task<IReadOnlyList<IDocumentCollection>> LoadCollectionsAsync(DataStorageConfiguration dataStorageConfiguration)
        {
            ConcurrentBag<IDocumentCollection> collections = new ConcurrentBag<IDocumentCollection>();
            //todo: logger
            _logger.LogDebug("Collection loading started");
            foreach (var configuration in dataStorageConfiguration.Collections)
            {
                var documentCollectionType = typeof(DocumentCollection<>).MakeGenericType(configuration.DocumentType);
                var documentCollection = (IDocumentCollection)Activator.CreateInstance(documentCollectionType, configuration);
                var addMethodInfo = documentCollectionType.GetMethod(nameof(IDocumentCollection<object>.Add), new[] { configuration.DocumentType });

                IEnumerable<Task<FileContent>> fileTasks;
                if (IsMultiFile(configuration))
                {
                    fileTasks = dataStorageConfiguration.StorageProvider.ReadDirectory(configuration.SourceFile);
                }
                else
                {
                    fileTasks = new[] { dataStorageConfiguration.StorageProvider.ReadFile(configuration.SourceFile) };
                }

                foreach (var fileTask in fileTasks)
                {
                    var content = await fileTask;

                    if (string.IsNullOrEmpty(content.Content))
                        continue;

                    var serializer = GetSerializer(configuration);

                    var objects = serializer.Deserialize(content, configuration.DocumentType);
                    foreach (var document in objects)
                    {
                        addMethodInfo.Invoke(documentCollection, new[] { document });
                    }
                }

                collections.Add(documentCollection);
                _logger.LogInformation($"Collection loaded. Type: {configuration.DocumentType}");
            }

            return collections.ToArray();
        }

        public async Task<IDictionary<Type, IEntityList>> LoadEntitiesAsync()
        {
            ConcurrentDictionary<Type, IEntityList> entityLists = new ConcurrentDictionary<Type, IEntityList>();

            _logger.LogDebug("Collection loading started");

            foreach (var documentType in _dataStore.Schema.DocumentTypes)
            {
                var loadEntityListMethod = GetType().GetMethod(nameof(LoadEntityList), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(documentType);
                var loadEntityListTask = (Task<IEntityList>)loadEntityListMethod.Invoke(this, null);
                var entityList = await loadEntityListTask;

                entityLists.TryAdd(documentType, entityList);
                _logger.LogInformation($"Collection loaded. Type: {documentType}");
            }

            return entityLists;
        }

        private async Task<IEntityList> LoadEntityList<T>() where T : class
        {
            DataStorageConfiguration storageConfig = _dataStorageConfiguration;
            Type documentType = typeof(T);

            var collectionConfig = storageConfig.Collections
                .FirstOrDefault(config => config.DocumentType == documentType);

            if (collectionConfig == null)
            {
                return new ReadOnlyEntityList<T>(new ExportData<T>());
            }

            var configurationTyped = collectionConfig.ToTyped<T>();
            var serializer = GetSerializer(collectionConfig);

            var fileTasks = GetFileTasks(collectionConfig, storageConfig);
            var fileContents = await ReadAllInOneThread(fileTasks);
            var entities = DeserializeEntitiesInOneThread<T>(fileContents, serializer);

            var isReadonly = storageConfig.IsReadOnly();
            var entityList = CreateEntityList<T>(isReadonly, entities, configurationTyped);
            return entityList;
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

        private IEnumerable<Task<FileContent>> GetFileTasks(CollectionConfiguration collectionConfig, DataStorageConfiguration storageConfig)
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
            CollectionConfigurationTyped<T> collectionConfig)
            where T : class
        {
            string GetKey(T item) => collectionConfig.KeyGetter.GetIdFunc()(item);

            var entityWithKeys = entities.Select(ent => new EntityWithKey<T>(ent, GetKey(ent))).ToList();

            if (readOnly)
            {
                return new ReadOnlyEntityList<T>(new ExportData<T> { Added = entityWithKeys });
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

        public ISerializer GetSerializer(CollectionConfiguration configuration)
        {
            return configuration.Serializer ?? _conventions.GetSerializer(configuration);
        }

        public bool IsMultiFile(CollectionConfiguration configuration)
        {
            var isDirectory = !Path.HasExtension(configuration.SourceFile);
            return isDirectory;
        }

        public void SaveCollection(IDocumentCollection collection)
        {
            GetType().GetMethod(nameof(SaveCollectionInternal), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(collection.Configuration.DocumentType)
                .Invoke(this, new[] { collection });
        }

        private void SaveCollectionInternal<T>(IDocumentCollection<T> collection) where T : class
        {
            var configuration = collection.Configuration;

            var serializer = GetSerializer(configuration);
            var serializerInfo = serializer.GetInfo();
            var items = collection.Find(arg => true).ToList();
            var collectionDir = configuration.SourceFile;

            if (IsMultiFile(configuration))
            {
                foreach (var item in items)
                {
                    var key = collection.GetKey(item);
                    var format = string.Format("{0}{1}", key, serializerInfo.Extension);
                    var fileName = Path.Combine(collectionDir, format);
                    SaveFiles(serializer, new[] { item }, fileName, configuration.DocumentType);
                }

                ProcessDeletes(collection, collectionDir, serializerInfo.Extension).GetAwaiter().GetResult();
            }
            else
            {
                //todo: do not rewrite same
                SaveFiles(serializer, items, configuration.SourceFile, configuration.DocumentType);
            }

            collection.HasChanges = false;
        }

        private void SaveFiles<T>(ISerializer serializer, IReadOnlyCollection<T> items, string fileName, Type configurationDocumentType) where T : class
        {
            var fileContent = serializer.Serialize(items, configurationDocumentType);
            _dataStorageConfiguration.StorageProvider.WriteFile(fileName, fileContent);
        }

        private async Task ProcessDeletes<T>(IDocumentCollection<T> collection, string collectionDir, string extention) where T : class
        {
            var commandLog = collection.GetCommandLog();
            var deleteCommands = commandLog.Where(command => !command.Processed && command.CommandType == CommandType.Delete);

            foreach (var deleteCommand in deleteCommands)
            {
                var fileName = Path.Combine(collectionDir, string.Format("{0}{1}", deleteCommand.Key, extention));

                await _dataStorageConfiguration.StorageProvider.DeleteFile(fileName);
                deleteCommand.Processed = true;
            }
        }
    }
}
