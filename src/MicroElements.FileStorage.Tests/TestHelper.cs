using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.KeyAccessors;
using MicroElements.FileStorage.KeyGenerators;
using MicroElements.FileStorage.StorageEngine;
using MicroElements.FileStorage.Tests.Models;

namespace MicroElements.FileStorage.Tests
{
    internal static class TestHelper
    {
        public static async Task<DataStore> CreateInMemoryDataStore()
        {
            var inMemoryStorageEngine = new InMemoryStorageProvider();
            await inMemoryStorageEngine.WriteFile("currencies.json", new FileContent("currencies.json", "[]"));
            DataStoreConfiguration storeConfiguration = new DataStoreConfiguration
            {
                Storages = new[]
                {
                   new DataStorageConfiguration
                   {
                       ReadOnly = false,
                       StorageProvider = inMemoryStorageEngine,
                       Collections = new CollectionConfiguration[]
                       {
                           new CollectionConfiguration<Currency>
                           {
                               DocumentType = typeof(Currency),
                               SourceFile = "currencies.json",
                               KeyGetter = new DefaultKeyAccessor<Currency>(nameof(Currency.Code)),
                           },
                       }
                   }
                }
            };
            var dataStore = new DataStore(storeConfiguration);

            await dataStore.Initialize();
            return dataStore;
        }
    }

    internal static class TestData
    {
        public static Person Bill => new Person
        {
            Id = "1",
            FirstName = "Bill",
            LastName = "Gates"
        };

        public static async Task<DataStore> CreatePersonsDataStore(IStorageProvider storageProvider = null)
        {
            var inMemoryStorageEngine = storageProvider ?? new InMemoryStorageProvider();
            DataStoreConfiguration storeConfiguration = new DataStoreConfiguration
            {
                Storages = new[]
                {
                    new DataStorageConfiguration
                    {
                        ReadOnly = false,
                        StorageProvider = inMemoryStorageEngine,
                        Collections = new CollectionConfiguration[]
                        {
                            new CollectionConfiguration<Person>
                            {
                                DocumentType = typeof(Person),
                                SourceFile = "persons",
                                KeyGetter = new DefaultKeyAccessor<Person>(),
                                KeyGenerator = new SemanticKeyGenerator<Person>(person => $"{person.FirstName}_{person.LastName}")
                            },
                        }
                    }
                }
            };
            var dataStore = new DataStore(storeConfiguration);

            await dataStore.Initialize();
            return dataStore;
        }
    }
}
