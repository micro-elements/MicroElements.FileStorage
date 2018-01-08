// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage
{
    public class DataStore : IDataStore
    {
        private readonly DataStoreConfiguration _configuration;
        private readonly List<IDocumentCollection> _collections = new List<IDocumentCollection>();

        public DataStore(DataStoreConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public async Task Initialize()
        {
            _configuration.Verify();
            //todo: logger
            foreach (var configuration in _configuration.Collections)
            {
                var documentCollectionType = typeof(DocumentCollection<>).MakeGenericType(configuration.DocumentType);
                var documentCollection = (IDocumentCollection)Activator.CreateInstance(documentCollectionType, configuration);
                var addMethodInfo = documentCollectionType.GetMethod(nameof(IDocumentCollection<object>.Add), new[] { configuration.DocumentType });

                _collections.Add(documentCollection);

                IEnumerable<Task<FileContent>> fileTasks;
                if (IsMultiFile(configuration))
                {
                    fileTasks = _configuration.StorageEngine.ReadDirectory(configuration.SourceFile);
                }
                else
                {
                    fileTasks = new[] { _configuration.StorageEngine.ReadFile(configuration.SourceFile) };
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
            }
        }

        /// <inheritdoc />
        public IDocumentCollection<T> GetCollection<T>() where T : class
        {
            var documentCollection = (IDocumentCollection<T>)_collections.FirstOrDefault(collection => collection is IDocumentCollection<T>);
            return documentCollection ?? throw new InvalidOperationException($"Document collection for type {typeof(T)} is not registered in data store.");
        }

        /// <inheritdoc />
        public IReadOnlyList<IDocumentCollection> GetCollections()
        {
            return _collections;
        }

        /// <inheritdoc />
        public void Save()
        {
            foreach (var collection in _collections)
            {
                if (collection.HasChanges)
                {
                    CallSaveCollection(collection);
                }
            }
        }

        /// <inheritdoc />
        public void Drop()
        {
            foreach (var collection in _collections)
            {
                collection.Drop();
            }
        }

        private void CallSaveCollection(IDocumentCollection collection)
        {
            GetType().GetMethod(nameof(SaveCollection), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(collection.Configuration.DocumentType)
                .Invoke(this, new[] { collection });
        }

        internal void SaveCollection<T>(IDocumentCollection<T> collection) where T : class
        {
            var serializer = GetSerializer(collection.Configuration);
            var items = collection.Find(arg => true).ToList();
            if (IsMultiFile(collection.Configuration))
            {
                foreach (var item in items)
                {
                    var fileContent = serializer.Serialize(new[] { item }, collection.Configuration.DocumentType);
                    var collectionDir = collection.Configuration.SourceFile;
                    var key = collection.GetKey(item);
                    var serializerInfo = serializer.GetInfo();
                    var fileName = Path.Combine(collectionDir, key + serializerInfo.Extension);
                    _configuration.StorageEngine.WriteFile(fileName, fileContent);
                }

            }
            else
            {
                var fileContent = serializer.Serialize(items, collection.Configuration.DocumentType);
                _configuration.StorageEngine.WriteFile(collection.Configuration.SourceFile, fileContent);
            }

            collection.HasChanges = false;

        }

        private ISerializer GetSerializer(CollectionConfiguration configuration)
        {
            return configuration.Serializer ?? _configuration.Conventions.GetSerializer(configuration);
        }

        private bool IsMultiFile(CollectionConfiguration configuration)
        {
            var isDirectory = !Path.HasExtension(configuration.SourceFile);
            return isDirectory;
        }
    }
}
