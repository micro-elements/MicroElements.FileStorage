using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.Experimental;
using MicroElements.FileStorage.KeyAccessors;
using MicroElements.FileStorage.KeyGenerators;
using MicroElements.FileStorage.Serializers;
using MicroElements.FileStorage.StorageEngine;
using MicroElements.FileStorage.Tests.Models;
using MicroElements.FileStorage.ZipEngine;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using JsonSerializer = MicroElements.FileStorage.Serializers.JsonSerializer;

namespace MicroElements.FileStorage.Tests
{
    public class FileStorageTests
    {
        [Theory]
        [InlineData(nameof(FileStorageProvider))]
        public async Task delete_should_delete_file_multifile_collection(string typeStorageEngine)
        {
            var basePath = Path.GetFullPath("TestData/DataStore/delete_multifile_collection");
            var collectionDir = "persons";
            var collectionFullDir = Path.Combine(basePath, collectionDir);
            if (Directory.Exists(collectionFullDir))
                Directory.Delete(collectionFullDir, true);

            var fileNameBill = "1.json";
            var fileNameSteve = "2.json";

            var storageEngine = GetStorageEngine(typeStorageEngine, basePath);
            Directory.CreateDirectory(basePath);
            var storeConfiguration = new DataStoreConfiguration
            {
                StorageProvider = storageEngine,
                Collections = new[]
                {
                    new CollectionConfigurationTyped<Person>
                    {
                        SourceFile = "persons",
                        Serializer = new JsonSerializer(),
                        KeyGetter = new DefaultKeyAccessor<Person>(),
                        KeyGenerator = new SemanticKeyGenerator<Person>(person => $"{person.FirstName}_{person.LastName}")
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
                Id = "1",
                FirstName = "Bill",
                LastName = "Gates"
            });
            collection.Add(new Person
            {
                Id = "2",
                FirstName = "Steve",
                LastName = "Ballmer"
            });
            collection.Count.Should().Be(2);

            dataStore.Save();
            var fileBill = Path.Combine(collectionFullDir, fileNameBill);
            var fileSteve = Path.Combine(collectionFullDir, fileNameSteve);

            File.Exists(fileBill).Should().BeTrue();
            File.Exists(fileSteve).Should().BeTrue();

            collection.Delete("1");
            File.Exists(fileBill).Should().BeTrue();
            dataStore.Save();
            File.Exists(fileBill).Should().BeFalse();
            File.Exists(fileSteve).Should().BeTrue();

            collection.Delete("2");
            File.Exists(fileSteve).Should().BeTrue();
            dataStore.Save();
            File.Exists(fileSteve).Should().BeFalse();

        }

        [Theory()]
        [InlineData(nameof(FileStorageProvider))]
        [InlineData(nameof(ZipStorageProvider))]
        public async Task load_single_file_collection(string typeStorageEngine)
        {
            var basePath = Path.GetFullPath("TestData/DataStore/SingleFileCollection");
            var storageEngine = GetStorageEngine(typeStorageEngine, basePath);

            var storeConfiguration = new DataStoreConfiguration
            {
                StorageProvider = storageEngine,
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

        private static IStorageProvider GetStorageEngine(string storageName, string basePath)
        {
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
            IStorageProvider storageProvider = null;
            switch (storageName)
            {
                case nameof(FileStorageProvider):
                    storageProvider = new FileStorageProvider(new FileStorageConfiguration { BasePath = basePath });
                    break;
                case nameof(ZipStorageProvider):
                    storageProvider = new ZipStorageProvider(new ZipStorageConfiguration(new MemoryStream()) { Mode = ZipStorageEngineMode.Write, LeaveOpen = true });
                    var dic = new DirectoryInfo(basePath);
                    var allFiles = dic.GetFiles("*", SearchOption.AllDirectories);
                    foreach (var file in allFiles)
                    {
                        var location = file.FullName.Replace(dic.FullName, "").TrimStart('/').TrimStart('\\');
                        var fileContent = new FileContent(location, File.ReadAllText(file.FullName));
                        storageProvider.WriteFile(location, fileContent).GetAwaiter().GetResult();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(storageName);
            }

            return storageProvider;
        }

        [Theory]
        [InlineData(nameof(FileStorageProvider), false)]
        [InlineData(nameof(FileStorageProvider), true)]
        [InlineData(nameof(ZipStorageProvider), false)]
        [InlineData(nameof(ZipStorageProvider), true)]
        public async Task load_multi_file_collection(string typeStorageEngine, bool relative)
        {
            var basePath = "TestData/DataStore/MultiFileCollection";
            if (!relative)
                basePath = Path.GetFullPath(basePath);

            var storageEngine = GetStorageEngine(typeStorageEngine, basePath);

            var storeConfiguration = new DataStoreConfiguration
            {
                StorageProvider = storageEngine,
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
        [InlineData(nameof(FileStorageProvider))]
        [InlineData(nameof(ZipStorageProvider))]
        public async Task load_csv_collection(string typeStorageEngine)
        {
            var basePath = Path.GetFullPath("TestData/DataStore/WithConvert");
            var storageEngine = GetStorageEngine(typeStorageEngine, basePath);

            var storeConfiguration = new DataStoreConfiguration
            {
                StorageProvider = storageEngine,
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
        [InlineData(nameof(FileStorageProvider))]
        [InlineData(nameof(ZipStorageProvider))]
        public async Task create_collection_and_save(string typeStorageEngine)
        {
            var basePath = Path.GetFullPath("TestData/DataStore/create_collection_and_save");
            var dataStore = GetPersonDataStore(typeStorageEngine, basePath);

            var collection = dataStore.GetCollection<Person>();
            collection.Should().NotBeNull();
            collection.Drop();
            //collection.Count.Should().Be(0);

            var person1 = new Person
            {
                FirstName = "Bill",
                LastName = "Gates"
            };
            collection.Add(person1);
            collection.Count.Should().Be(1);

            var person2 = collection.Find(p => true).First();
            person2.Id.Should().NotBeNullOrEmpty("Id must be generated");

            dataStore.Save();

        }

        [Theory()]
        [InlineData(nameof(FileStorageProvider))]
        //[InlineData(nameof(ZipStorageProvider))]//todo: doesnot work
        public void save_update_save(string typeStorageEngine)
        {
            string basePath = Path.GetFullPath("TestData/DataStore/save_update_save");
            var dataStore = GetPersonDataStore(typeStorageEngine, basePath);

            var collection = dataStore.GetCollection<Person>();

            var person1 = new Person
            {
                FirstName = "Bill_123456789",
                LastName = "Gates"
            };
            collection.Add(person1);
            collection.Count.Should().Be(1);

            // First save
            dataStore.Save();

            var personId = person1.Id;
            var person2 = collection.Get(personId);
            person1.FirstName.Should().Be("Bill_123456789");
            person2.FirstName = "Bill_123";
            collection.HasChanges = true;
            dataStore.Save();

            dataStore = GetPersonDataStore(typeStorageEngine, basePath, delete: false);
            collection = dataStore.GetCollection<Person>();
            var person3 = collection.Get(personId);
            person3.FirstName.Should().Be("Bill_123");
        }

        private static DataStore GetPersonDataStore(string typeStorageEngine, string basePath, bool delete = true)
        {
            var file = Path.Combine(basePath, "persons.json");

            if (delete && File.Exists(file))
                File.Delete(file);

            var storageEngine = GetStorageEngine(typeStorageEngine, basePath);

            Directory.CreateDirectory(basePath);
            var storeConfiguration = new DataStoreConfiguration
            {
                StorageProvider = storageEngine,
                Collections = new[]
                {
                    new CollectionConfigurationTyped<Person>()
                    {
                        Name = "Persons",
                        SourceFile = "persons.json",
                        Format = "json",
                        Version = "1.0",
                        Serializer = new JsonSerializer(),
                        OneFilePerCollection = true,
                    },
                }
            };
            var dataStore = new DataStore(storeConfiguration);

            dataStore.Initialize().GetAwaiter().GetResult();

            return dataStore;
        }

        [Theory()]
        [InlineData(nameof(FileStorageProvider))]
        [InlineData(nameof(ZipStorageProvider))]
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
                StorageProvider = storageEngine,
                Collections = new[]
                {
                    new CollectionConfigurationTyped<Person>
                    {
                        SourceFile = "persons",
                        Serializer = new JsonSerializer(),
                        KeyGetter = new DefaultKeyAccessor<Person>(),
                        KeyGenerator = new SemanticKeyGenerator<Person>(person => $"{person.FirstName}_{person.LastName}")
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
                case nameof(FileStorageProvider):
                    var fileBill = Path.Combine(collectionFullDir, fileNameBill);
                    var fileSteve = Path.Combine(collectionFullDir, fileNameSteve);

                    File.Exists(fileBill).Should().BeTrue();
                    File.Exists(fileSteve).Should().BeTrue();

                    fileAllTextBill = File.ReadAllText(fileBill);
                    break;
                case nameof(ZipStorageProvider):
                    var zipStorageEngine = storageEngine as ZipStorageProvider;
                    zipStorageEngine.Should().NotBeNull();
                    var zipArchive = zipStorageEngine.GetZipArchive();
                    using (var billStream = zipArchive.GetEntry(collectionDir + "/" + fileNameBill).Open())
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
        [InlineData(nameof(FileStorageProvider))]
        [InlineData(nameof(ZipStorageProvider))]
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
                StorageProvider = storageEngine,
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
                StorageProvider = new FileStorageProvider(new FileStorageConfiguration() { BasePath = basePath }),
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

        [Fact]
        public async void double_add_with_same_id_should_update_item()
        {
            var dataStore = await TestHelper.CreateInMemoryDataStore();

            var collection = dataStore.GetCollection<Currency>();
            collection.Should().NotBeNull();

            collection.Add(new Currency { Code = "USD", Name = "Dollar" });
            collection.Count.Should().Be(1);
            collection.Get("USD").Name.Should().Be("Dollar");

            collection.Add(new Currency { Code = "USD", Name = "Dollar updated" });
            collection.Count.Should().Be(1);
            collection.Get("USD").Name.Should().Be("Dollar updated");
        }

        [Fact]
        public async Task add_get_with_identity_key()
        {
            var basePath = Path.GetFullPath("TestData/DataStore/add_get_with_identity_key");
            var file = Path.Combine(basePath, "entities.json");
            if (File.Exists(file))
                File.Delete(file);

            Directory.CreateDirectory(basePath);
            var storeConfiguration = new DataStoreConfiguration
            {
                Storages = new[]
                {
                    new DataStorageConfiguration
                    {
                        ReadOnly = false,
                        StorageProvider = new FileStorageProvider(new FileStorageConfiguration() { BasePath = basePath }),
                        Collections = new[]
                        {
                            new CollectionConfigurationTyped<Person>
                            {
                                Name = "entities",
                                SourceFile = "entities.json",
                                KeyGetter = new DefaultKeyAccessor<Person>(nameof(Person.Id)),
                                KeySetter = new DefaultKeyAccessor<Person>(nameof(Person.Id)),
                                KeyGenerator = new IdentityKeyGenerator<Person>(1, true)
                            },
                        }
                    }
                }
            };
            var dataStore = new DataStore(storeConfiguration);
            await dataStore.Initialize();

            var entityWithIntId = new Person() { LastName = "SomeName" };
            using (var session = dataStore.OpenSession())
                session.AddOrUpdate(entityWithIntId);

            //
            //collection.Add(entityWithIntId);
            entityWithIntId.Id.Should().Be("entities/1");

            var collection = dataStore.GetCollection<Person>();
            var getResult = collection.Get("entities/1");
            getResult.Should().NotBeNull();

            var item = new Person { LastName = "Name2" };
            collection.Add(item);
            item.Id.Should().Be("entities/2");

            var storeConfiguration2 = new DataStoreConfiguration
            {
                Storages = new[]
                {
                    new DataStorageConfiguration
                    {
                        ReadOnly = false,
                        StorageProvider = new FileStorageProvider(new FileStorageConfiguration() { BasePath = basePath }),
                        Collections = new[]
                        {
                            new CollectionConfigurationTyped<Person>
                            {
                                Name = "entities",
                                SourceFile = "entities.json",
                                KeyGetter = new DefaultKeyAccessor<Person>(nameof(Person.Id)),
                                KeySetter = new DefaultKeyAccessor<Person>(nameof(Person.Id)),
                                KeyGenerator = new IdentityKeyGenerator<Person>(1, false)
                            },
                        }
                    }
                }
            };
            var dataStore2 = new DataStore(storeConfiguration2);
            await dataStore2.Initialize();

            var entityWithIntId2 = new Person() { FirstName = "SomeName" };
            var collection2 = dataStore2.GetCollection<Person>();
            collection2.Add(entityWithIntId2);
            entityWithIntId2.Id.Should().Be("1");

            var getResult2 = collection2.Get("1");
            getResult2.Should().NotBeNull();

            var item2 = new Person { LastName = "Name2" };
            collection2.Add(item2);
            item2.Id.Should().Be("2");
        }
    }
}
