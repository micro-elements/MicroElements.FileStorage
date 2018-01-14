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
        [Fact]
        public async Task load_single_file_collection()
        {
            var basePath = Path.GetFullPath("TestData/DataStore/SingleFileCollection");
            var storeConfiguration = new DataStoreConfiguration
            {
                BasePath = basePath,
                StorageEngine = new FileStorageEngine(basePath),
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

        [Fact]
        public async Task load_multi_file_collection()
        {
            var basePath = Path.GetFullPath("TestData/DataStore/MultiFileCollection");
            var storeConfiguration = new DataStoreConfiguration
            {
                BasePath = basePath,
                StorageEngine = new FileStorageEngine(basePath),
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

        [Fact]
        public async Task load_csv_collection()
        {
            var basePath = Path.GetFullPath("TestData/DataStore/WithConvert");
            var storeConfiguration = new DataStoreConfiguration
            {
                BasePath = basePath,
                StorageEngine = new FileStorageEngine(basePath),
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

        [Fact]
        public async Task create_collection_and_save()
        {
            var basePath = Path.GetFullPath("TestData/DataStore/create_collection_and_save");
            var file = Path.Combine(basePath, "persons.json");
            if (File.Exists(file))
                File.Delete(file);

            Directory.CreateDirectory(basePath);
            var storeConfiguration = new DataStoreConfiguration
            {
                BasePath = basePath,
                StorageEngine = new FileStorageEngine(basePath), //todo: DI
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

        [Fact]
        public async Task save_multifile_collection_with_semantic_keys()
        {
            var basePath = Path.GetFullPath("TestData/DataStore/save_multifile_collection");
            var collectionDir = Path.Combine(basePath, "persons");
            if (Directory.Exists(collectionDir))
                Directory.Delete(collectionDir, true);

            Directory.CreateDirectory(basePath);
            var storeConfiguration = new DataStoreConfiguration
            {
                BasePath = basePath,
                StorageEngine = new FileStorageEngine(basePath),
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

            var file1 = Path.Combine(collectionDir, "Bill_Gates.json");
            var file2 = Path.Combine(collectionDir, "Steve_Ballmer.json");

            File.Exists(file1).Should().BeTrue();
            File.Exists(file2).Should().BeTrue();

            var bill = JsonConvert.DeserializeObject<Person>(File.ReadAllText(file1));
            bill.Should().BeEquivalentTo(new Person
            {
                Id = "Bill_Gates",
                FirstName = "Bill",
                LastName = "Gates"
            });
        }

        [Fact]
        public async Task key_generation()
        {
            var basePath = Path.GetFullPath("TestData/DataStore/key_generation");
            var file = Path.Combine(basePath, "persons.json");
            if (File.Exists(file))
                File.Delete(file);

            Directory.CreateDirectory(basePath);
            var storeConfiguration = new DataStoreConfiguration
            {
                BasePath = basePath,
                StorageEngine = new FileStorageEngine(basePath),
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
