// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.FileStorage.StorageEngine
{
    //todo: persistent data store configuration
    public interface IStorageConfiguration
    {
        string Name { get; }
        int Order { get; }
        bool IsActive { get; }
        string[] Types { get; }
        bool IsDefault { get; }
    }
}
