// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.FileStorage.Abstractions
{
    public static class StorageEngineExtensions
    {
        public static bool IsExists(this IStorageProvider storageProvider, string location)
        {
            return storageProvider.GetFileMetadata(location).IsExists;
        }
    }
}
