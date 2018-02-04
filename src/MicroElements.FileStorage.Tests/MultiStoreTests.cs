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
            var storeConfiguration = new DataStoreConfiguration
            {
                Storages = new[]
                {
                    new DataStorageConfiguration
                    {
                        StorageProvider = new FileStorageProvider(new FileStorageConfiguration
                        {
                            BasePath = Path.GetFullPath("TestData/MultiStore/Snapshot"),
                            IsReadOnly = true
                        }),
                        Collections = new[]
                        {
                            new CollectionConfigurationTyped<Person>
                            {
                                SourceFile = "Persons.json",
                            },
                        }
                    },
                    new DataStorageConfiguration
                    {
                        StorageProvider = new FileStorageProvider(new FileStorageConfiguration
                        {
                            BasePath = Path.GetFullPath("TestData/MultiStore/Addon"),
                            IsReadOnly = false
                        }),
                        Collections = new[]
                        {
                            new CollectionConfigurationTyped<Person>
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

            using (var session = dataStore.OpenSession())
            {
                var person = new Person { FirstName = "FirstNameFromAddon", LastName = "LastNameFromAddon" };
                session.AddOrUpdate(person);
            }
        }
    }
}
