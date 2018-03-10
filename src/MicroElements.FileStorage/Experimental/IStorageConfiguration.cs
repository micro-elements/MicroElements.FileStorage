// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using JetBrains.Annotations;
using MicroElements.FileStorage.Abstractions;

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

    /// <summary>
    /// DataStore registry.
    /// </summary>
    public interface IDataStoreRegistry
    {
        /// <summary>
        /// Gets <see cref="IDataStore"/> for type.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <returns><see cref="IDataStore"/> or null if not found.</returns>
        [CanBeNull] IDataStore GetDataStore([NotNull] Type entityType);
    }
}
