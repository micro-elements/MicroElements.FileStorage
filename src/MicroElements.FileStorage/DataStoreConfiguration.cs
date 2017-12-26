using MicroElements.FileStorage.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MicroElements.FileStorage
{
    /// <summary>
    /// DataStore configuration.
    /// </summary>
    public class DataStoreConfiguration
    {
        /// <summary>
        /// Base path to file storage.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// StorageEngine.
        /// </summary>
        public IStorageEngine StorageEngine { get; set; }

        /// <summary>
        /// Collection definitions.
        /// </summary>
        public CollectionConfiguration[] Collections { get; set; }

        //todo: service collection
        public ILoggerFactory LoggerFactory { get; set; } = NullLoggerFactory.Instance;

        public Conventions Conventions { get; set; } = Conventions.Default;
    }
}