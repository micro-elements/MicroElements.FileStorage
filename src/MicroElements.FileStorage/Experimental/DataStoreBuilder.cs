// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.Experimental
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
