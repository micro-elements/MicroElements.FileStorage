using System.Linq;
using MicroElements.Bootstrap;
using MicroElements.FileStorage.NuGetEngine;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using IDataStoreConfiguration = MicroElements.FileStorage.PersistentConfiguration.IDataStoreConfiguration;

namespace MicroElements.FileStorage.Tests
{
    public class ConfigurationTests
    {
        [Fact]
        public void ReadDataStore1Config()
        {
            var buildContext = new ApplicationBuilder().Build(
                new StartupConfiguration
                {
                    ConfigurationPath = @"TestData/Configuration",
                });
            var service1 = buildContext.ServiceProvider.GetService<IDataStoreConfiguration>();
            var nuGetStorageConfiguration = buildContext.ServiceProvider.GetService<NuGetStorageConfiguration>();
            var nuGetStorageConfiguration2 = buildContext.ServiceProvider.GetService<INuGetStorageConfiguration>();
            var nuGetStorageConfiguration3 = buildContext.ServiceProvider.GetServices<INuGetStorageConfiguration>().ToList();
            int i = 0;
        }
    }
}
