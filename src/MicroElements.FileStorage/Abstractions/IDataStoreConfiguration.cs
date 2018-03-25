// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// DataStore configuration.
    /// </summary>
    public interface IDataStoreConfiguration
    {
        /// <summary>
        /// Gets a value indicating whether the DataStore is readonly.
        /// </summary>
        bool ReadOnly { get; }

        /// <summary>
        /// Gets storage configurations.
        /// </summary>
        IReadOnlyList<IDataStorageConfiguration> Storages { get; }

        /// <summary>
        /// Gets DataStore conventions.
        /// </summary>
        Conventions Conventions { get; }
    }
}
