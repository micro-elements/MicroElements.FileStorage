using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.KeyAccessors;
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
                           new CollectionConfigurationTyped<Currency>
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
}
