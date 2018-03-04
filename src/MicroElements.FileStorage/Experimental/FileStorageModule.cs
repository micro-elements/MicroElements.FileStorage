// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MicroElements.FileStorage.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MicroElements.FileStorage.Experimental
{
    public class FileStorageModule
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IDocumentCollection<>), typeof(DocumentCollection<>));
        }
    }
}
