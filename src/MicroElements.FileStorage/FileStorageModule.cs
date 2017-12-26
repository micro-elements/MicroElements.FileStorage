using MicroElements.FileStorage.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MicroElements.FileStorage
{
    public class FileStorageModule
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IDocumentCollection<>), typeof(DocumentCollection<>));
            services.AddSingleton(typeof(IDocumentCollectionFactory), typeof(DocumentCollectionFactory));

        }
    }
}