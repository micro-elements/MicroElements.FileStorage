using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.StorageEngine;
using MicroElements.FileStorage.Tests.Models;
using Xunit;

namespace MicroElements.FileStorage.Tests
{
    public class MultiStoreTests
    {
        [Fact]
        public async Task create_multistore_test()
        {
            var addonPersonsJson = "TestData/MultiStore/Addon/Persons.json";
            if (File.Exists(addonPersonsJson))
                File.Delete(addonPersonsJson);

            var storeConfiguration = new DataStoreConfiguration
            {
                Storages = new[]
                {
                    new DataStorageConfiguration
                    {
                        ReadOnly = true,
                        StorageProvider = new FileStorageProvider(new FileStorageConfiguration
                        {
                            BasePath = Path.GetFullPath("TestData/MultiStore/Snapshot")
                        }),
                        Collections = new ICollectionConfiguration[]
                        {
                            new CollectionConfiguration<Person>
                            {
                                SourceFile = "Persons.json",
                            },
                        }
                    },
                    new DataStorageConfiguration
                    {
                        ReadOnly = false,
                        StorageProvider = new FileStorageProvider(new FileStorageConfiguration
                        {
                            BasePath = Path.GetFullPath("TestData/MultiStore/Addon"),
                        }),
                        Collections = new ICollectionConfiguration[]
                        {
                            new CollectionConfiguration<Person>
                            {
                                SourceFile = "Persons.json",
                            },
                        }
                    }
                }
            };
            var dataStore = new DataStore(storeConfiguration);

            await dataStore.Initialize();

            var collection = dataStore.GetCollection<Person>();
            collection.Should().NotBeNull();
            collection.Count.Should().Be(2);

            var person1 = collection.Get("1");
            person1.Should().NotBeNull();

            using (var session = dataStore.OpenSession())
            {
                var person = new Person { FirstName = "FirstNameFromAddon", LastName = "LastNameFromAddon" };
                session.AddOrUpdate(person);
            }
        }
    }
}
