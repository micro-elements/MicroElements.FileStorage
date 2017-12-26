using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            //todo: logger
            foreach (var configuration in _configuration.Collections)
            {
                var documentCollectionType = typeof(DocumentCollection<>).MakeGenericType(configuration.DocumentType);
                var documentCollection = (IDocumentCollection)Activator.CreateInstance(documentCollectionType, configuration);
                var addMethodInfo = documentCollectionType.GetMethod(nameof(IDocumentCollection<object>.Add), new[] { configuration.DocumentType });

                _collections.Add(documentCollection);

                IEnumerable<Task<FileContent>> fileTasks;
                var isDirectory = !Path.HasExtension(configuration.SourceFile);
                if (isDirectory)
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

                    if (String.IsNullOrEmpty(content.Content))
                        continue;

                    var serializer = configuration.Serializer ?? _configuration.Conventions.GetSerializer(configuration);

                    var objects = serializer.Deserialize(content, configuration.DocumentType);
                    foreach (var document in objects)
                    {
                        addMethodInfo.Invoke(documentCollection, new[] { document });
                    }
                }
            }
        }

        public IDocumentCollection<T> GetCollection<T>() where T : class
        {
            return (IDocumentCollection<T>)_collections.First(collection => collection is IDocumentCollection<T>);
        }

        public void Save()
        {
            foreach (var collection in _collections)
            {
                if (collection.HasChanges)
                {
                    var configurationName = collection.Configuration.Name;

                    var serializer = collection.Configuration.Serializer;
                    var items = collection.GetAll();
                    var fileContent = serializer.Serialize(items, collection.Configuration.DocumentType);
                    //fileContent.Location = collection.Configuration.SourceFile;
                    Directory.CreateDirectory(_configuration.BasePath);
                    _configuration.StorageEngine.WriteFile(Path.Combine(_configuration.BasePath, collection.Configuration.SourceFile), fileContent);
                }
            }
        }
    }
}