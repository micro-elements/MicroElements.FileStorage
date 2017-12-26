using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage
{
    //todo: ServiceCollection build and inject
    public class DataStoreBuilder
    {
        public IDataStore Build()
        {
            return new DataStore(new DataStoreConfiguration());
        }
    }
}