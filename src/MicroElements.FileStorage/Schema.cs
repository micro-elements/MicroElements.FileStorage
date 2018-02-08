// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroElements.FileStorage
{
    public class Schema
    {
        private readonly DataStoreConfiguration _configuration;

        public DataStoreConfiguration Configuration => _configuration;

        public IReadOnlyList<Type> DocumentTypes { get; }

        public Schema(DataStoreConfiguration configuration)
        {
            _configuration = configuration;

            DocumentTypes = _configuration.Storages.SelectMany(storageConfig => storageConfig.Collections)
                .Select(collectionConfig => collectionConfig.DocumentType)
                .Distinct()
                .ToList();
        }
    }
}
