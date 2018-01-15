using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.Experimental;
using MicroElements.FileStorage.KeyGenerators;
using MicroElements.FileStorage.Serializers;
using MicroElements.FileStorage.StorageEngine;
using MicroElements.FileStorage.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using JsonSerializer = MicroElements.FileStorage.Serializers.JsonSerializer;

namespace MicroElements.FileStorage.Tests
{
    public class FileStorageTests
    {
        [Theory()]
        [InlineData(nameof(FileStorageEngine))]
        [InlineData(nameof(ZipStorageEngine))]
        public async Task load_single_file_collection(string typeStorageEngine)
        {
            var basePath = Path.GetFullPath("TestData/DataStore/SingleFileCollection");
            var storageEngine = GetStorageEngine(typeStorageEngine, basePath);

            var storeConfiguration = new DataStoreConfiguration
            {
                BasePath = basePath,
                StorageEngine = storageEngine,
                Collections = new[]
                {
                    new CollectionConfiguration
                    {
                        Name = "Persons",
                        DocumentType = typeof(Person),
                        SourceFile = "Persons.json",
                        Format = "json",
                        Version = "1.0"
                    },
                }
            };
            var dataStore = new DataStore(storeConfiguration);

            await dataStore.Initialize();

            dataStore.GetCollection<Person>().Should().BeSameAs(dataStore.GetCollection<Person>());

            var collection = dataStore.GetCollection<Person>();
            collection.Should().NotBeNull();
            collection.Count.Should().Be(2);

            CheckPersons(collection);
        }

        private static void CheckPersons(IDocumentCollection<Person> collection)
        {
            var person = collection.Find(_ => _.Id == "2").First();
            person.Should().NotBeNull();
            person.FirstName.Should().Be("Steve");
            person.LastName.Should().Be("Ballmer");

            person = collection.Get("1");
            person.Should().NotBeNull();
            person.FirstName.Should().Be("Bill");
            person.LastName.Should().Be("Gates");
        }

        private static IStorageEngine GetStorageEngine(string storageName, string basePath)
        {
            IStorageEngine storageEngine = null;
            switch (storageName)
            {
                case nameof(FileStorageEngine):
                    storageEngine = new FileStorageEngine(basePath);
                    break;
                case nameof(ZipStorageEngine):
                    storageEngine = new ZipStorageEngine(new MemoryStream(), ZipStorageEngineMode.Write, true);
                    var dic = new DirectoryInfo(basePath);
                    var allFiles = dic.GetFiles("*", SearchOption.AllDirectories);
                    foreach (var file in allFiles)
                    {
                        var location = file.FullName.Replace(dic.FullName, "").TrimStart('/').TrimStart('\\');
                        var fileContent = new FileContent(location, File.ReadAllText(file.FullName));
                        storageEngine.WriteFile(location, fileContent).GetAwaiter().GetResult();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(storageName);
            }

            return storageEngine;
        }

        [Theory()]
        [InlineData(nameof(FileStorageEngine))]
        [InlineData(nameof(ZipStorageEngine))]
        public async Task load_multi_file_collection(string typeStorageEngine)
        {
            var basePath = Path.GetFullPath("TestData/DataStore/MultiFileCollection");
            var storageEngine = GetStorageEngine(typeStorageEngine, basePath);

            var storeConfiguration = new DataStoreConfiguration
            {
                BasePath = basePath,
                StorageEngine = storageEngine,
                Collections = new[]
                {
                    new CollectionConfiguration
                    {
                        Name = "Persons",
                        DocumentType = typeof(Person),
                        SourceFile = "persons",
                        Format = "json",
                        Version = "1.0"
                    },
                }
            };
            var dataStore = new DataStore(storeConfiguration);

            await dataStore.Initialize();

            var collection = dataStore.GetCollection<Person>();
            collection.Should().NotBeNull();
            collection.Count.Should().Be(2);
        }

        [Theory()]
        [InlineData(nameof(FileStorageEngine))]
        [InlineData(nameof(ZipStorageEngine))]
        public async Task load_csv_collection(string typeStorageEngine)
        {
            var basePath = Path.GetFullPath("TestData/DataStore/WithConvert");
            var storageEngine = GetStorageEngine(typeStorageEngine, basePath);

            var storeConfiguration = new DataStoreConfiguration
            {
                BasePath = basePath,
                StorageEngine = storageEngine,
                Collections = new[]
                {
                    new CollectionConfiguration
                    {
                        DocumentType = typeof(Person),
                        SourceFile = "persons.csv",
                        Format = "csv",
                        Serializer = new SimpleCsvSerializer() //todo: привязать к полю Format или к расширению
                    },
                }
            };
            var dataStore = new DataStore(storeConfiguration);

            await dataStore.Initialize();

            var collection = dataStore.GetCollection<Person>();
            collection.Should().NotBeNull();
            collection.Count.Should().Be(2);

            CheckPersons(collection);
        }

        [Theory()]
        [InlineData(nameof(FileStorageEngine))]
        [InlineData(nameof(ZipStorageEngine))]
        public async Task create_collection_and_save(string typeStorageEngine)
        {
            var basePath = Path.GetFullPath("TestData/DataStore/create_collection_and_save");
            var file = Path.Combine(basePath, "persons.json");
            if (File.Exists(file))
                File.Delete(file);

            var storageEngine = GetStorageEngine(typeStorageEngine, basePath);

            Directory.CreateDirectory(basePath);
            var storeConfiguration = new DataStoreConfiguration
            {
                BasePath = basePath,
                StorageEngine = storageEngine,//todo: DI
                Collections = new[]
                {
                    new CollectionConfiguration
                    {
                        Name = "Persons",
                        DocumentType = typeof(Person),
                        SourceFile = "persons.json",
                        Format = "json",
                        Version = "1.0",
                        Serializer = new JsonSerializer(),
                        OneFilePerCollection = true
                    },
                }
            };
            var dataStore = new DataStore(storeConfiguration);

            await dataStore.Initialize();

            var collection = dataStore.GetCollection<Person>();
            collection.Should().NotBeNull();
            collection.Drop();
            //collection.Count.Should().Be(0);

            collection.Add(new Person
            {
                FirstName = "Bill",
                LastName = "Gates"
            });
            collection.Count.Should().Be(1);

            var person = collection.Find(p => true).First();
            person.Id.Should().NotBeNullOrEmpty("Id must be generated");

            dataStore.Save();
        }

        [Theory()]
        [InlineData(nameof(FileStorageEngine))]
        [InlineData(nameof(ZipStorageEngine))]
        public async Task save_multifile_collection_with_semantic_keys(string typeStorageEngine)
        {
            var basePath = Path.GetFullPath("TestData/DataStore/save_multifile_collection");
            var collectionDir = "persons";
            var collectionFullDir = Path.Combine(basePath, collectionDir);
            if (Directory.Exists(collectionFullDir))
                Directory.Delete(collectionFullDir, true);

            var fileNameBill = "Bill_Gates.json";
            var fileNameSteve = "Steve_Ballmer.json";

            var storageEngine = GetStorageEngine(typeStorageEngine, basePath);
            Directory.CreateDirectory(basePath);
            var storeConfiguration = new DataStoreConfiguration
            {
                BasePath = basePath,
                StorageEngine = storageEngine,
                Collections = new[]
                {
                    new CollectionConfigurationTyped<Person>
                    {
                        SourceFile = "persons",
                        Serializer = new JsonSerializer(),
                        KeyGetter = new DefaultKeyAccessor<Person>(),
                        KeyGenerator =
                            new SemanticKeyGenerator<Person>(person => $"{person.FirstName}_{person.LastName}")
                    },
                }
            };
            var dataStore = new DataStore(storeConfiguration);

            await dataStore.Initialize();

            var collection = dataStore.GetCollection<Person>();
            collection.Should().NotBeNull();
            collection.Count.Should().Be(0);

            collection.Add(new Person
            {
                FirstName = "Bill",
                LastName = "Gates"
            });
            collection.Add(new Person
            {
                FirstName = "Steve",
                LastName = "Ballmer"
            });
            collection.Count.Should().Be(2);

            dataStore.Save();

            var fileAllTextBill = string.Empty;

            switch (typeStorageEngine)
            {
                case nameof(FileStorageEngine):
                    var fileBill = Path.Combine(collectionFullDir, fileNameBill);
                    var fileSteve = Path.Combine(collectionFullDir, fileNameSteve);

                    File.Exists(fileBill).Should().BeTrue();
                    File.Exists(fileSteve).Should().BeTrue();

                    fileAllTextBill = File.ReadAllText(fileBill);
                    break;
                case nameof(ZipStorageEngine):
                    var zipStorageEngine = storageEngine as ZipStorageEngine;
                    zipStorageEngine.Should().NotBeNull();
                    var zipArchive = zipStorageEngine.GetZipArchiveReadOnlyAndDispose();
                    using (var billStream = zipArchive.GetEntry(Path.Combine(collectionDir, fileNameBill)).Open())
                    {
                        using (var billStreamReader = new StreamReader(billStream))
                        {
                            fileAllTextBill = billStreamReader.ReadToEnd();
                        }
                    }
                    zipArchive.GetEntry(Path.Combine(collectionDir, fileNameSteve));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var bill = JsonConvert.DeserializeObject<Person>(fileAllTextBill);
            bill.Should().BeEquivalentTo(new Person
            {
                Id = "Bill_Gates",
                FirstName = "Bill",
                LastName = "Gates"
            });
        }

        [Theory()]
        [InlineData(nameof(FileStorageEngine))]
        [InlineData(nameof(ZipStorageEngine))]
        public async Task key_generation(string typeStorageEngine)
        {
            var basePath = Path.GetFullPath("TestData/DataStore/key_generation");
            var file = Path.Combine(basePath, "persons.json");
            if (File.Exists(file))
                File.Delete(file);

            var storageEngine = GetStorageEngine(typeStorageEngine, basePath);

            Directory.CreateDirectory(basePath);
            var storeConfiguration = new DataStoreConfiguration
            {
                BasePath = basePath,
                StorageEngine = storageEngine,
                Collections = new[]
                {
                    new CollectionConfigurationTyped<Person>
                    {
                        DocumentType = typeof(Person),
                        SourceFile = "persons.json",
                        KeyGenerator = new IdentityKeyGenerator<Person>()
                    },
                }
            };
            var dataStore = new DataStore(storeConfiguration);

            await dataStore.Initialize();

            var collection = dataStore.GetCollection<Person>();
            collection.Should().NotBeNull();
            collection.Count.Should().Be(0);

            var person = new Person
            {
                FirstName = "Bill",
                LastName = "Gates"
            };
            collection.Add(person);
            person.Id.Should().Be("person/1");

            person = new Person
            {
                FirstName = "Steve",
                LastName = "Ballmer"
            };
            collection.Add(person);
            person.Id.Should().Be("person/2");


            dataStore.Save();
        }

        [Fact]
        public async Task not_standard_id_collection()
        {
            var basePath = Path.GetFullPath("TestData/DataStore/not_standard_id_collection");
            var file = Path.Combine(basePath, "currencies.json");
            if (File.Exists(file))
                File.Delete(file);

            Directory.CreateDirectory(basePath);
            var storeConfiguration = new DataStoreConfiguration
            {
                BasePath = basePath,
                //StorageEngine = new FileStorageEngine(basePath),
                Collections = new[]
                {
                    new CollectionConfigurationTyped<Currency>
                    {
                        DocumentType = typeof(Currency),
                        SourceFile = "currencies.json",
                        KeyGetter = new DefaultKeyAccessor<Currency>(nameof(Currency.Code)),

                    },
                }
            };
            var dataStore = new DataStore(storeConfiguration);

            await dataStore.Initialize();

            var collection = dataStore.GetCollection<Currency>();
            collection.Should().NotBeNull();


            collection.Add(new Currency()
            {
                Code = "USD",
                Name = "Dollar"
            });
            collection.Add(new Currency()
            {
                Code = "EUR",
                Name = "Euro"
            });
            collection.Count.Should().Be(2);

            dataStore.Save();
        }

        [Fact]
        public void builder_tests()
        {
            var services = new ServiceCollection();
            new FileStorageModule().ConfigureServices(services);

            services.AddSingleton(new CollectionConfiguration()
            {
                Name = "col1"
            });
            services.AddSingleton(new CollectionConfiguration()
            {
                Name = "col2"
            });
            var serviceProvider = services.BuildServiceProvider(true);
            var collectionConfigurations = serviceProvider.GetService<IEnumerable<CollectionConfiguration>>();
            var documentCollection = serviceProvider.GetRequiredService<IDocumentCollection<Person>>();
        }

        [Fact]
        public async void delete_should_delete_entity()
        {
            var dataStore = await TestHelper.CreateInMemoryDataStore();

            var collection = dataStore.GetCollection<Currency>();
            collection.Should().NotBeNull();

            collection.Add(new Currency { Code = "USD", Name = "Dollar" });
            collection.Add(new Currency { Code = "EUR", Name = "Euro" });
            collection.Count.Should().Be(2);

            collection.IsExists("USD").Should().BeTrue();
            collection.IsExists("EUR").Should().BeTrue();

            collection.Delete("USD");
            collection.Count.Should().Be(1);
            collection.IsExists("USD").Should().BeFalse();
            collection.IsExists("EUR").Should().BeTrue();

            // Delete not existent currency...
            collection.Delete("Bitcoin");
            collection.Count.Should().Be(1);

            ((Action)(() => collection.Delete(null))).Should().Throw<ArgumentNullException>();
        }
    }
}
