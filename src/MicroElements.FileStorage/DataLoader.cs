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
        private Conventions _conventions;
        private ILogger _logger;
        private readonly DataStorageConfiguration _dataStorageConfiguration;

        /// <inheritdoc />
        public DataLoader(Conventions conventions, ILogger logger, DataStorageConfiguration dataStorageConfiguration)
        {
            _conventions = conventions;
            _logger = logger;
            _dataStorageConfiguration = dataStorageConfiguration;
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
            var serializer = GetSerializer(collection.Configuration);
            var serializerInfo = serializer.GetInfo();
            var items = collection.Find(arg => true).ToList();
            var collectionDir = collection.Configuration.SourceFile;

            if (IsMultiFile(collection.Configuration))
            {
                foreach (var item in items)
                {
                    var key = collection.GetKey(item);
                    var format = string.Format("{0}{1}", key, serializerInfo.Extension);
                    var fileName = Path.Combine(collectionDir, format);
                    SaveFiles(serializer, new[] { item }, fileName, collection.Configuration.DocumentType);
                }

                ProcessDeletes(collection, collectionDir, serializerInfo.Extension).GetAwaiter().GetResult();
            }
            else
            {
                //todo: do not rewrite same
                SaveFiles(serializer, items, collection.Configuration.SourceFile, collection.Configuration.DocumentType);
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
