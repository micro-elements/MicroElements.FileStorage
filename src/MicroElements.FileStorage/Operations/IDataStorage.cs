// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.Operations
{
    // todo: Name, Key, Hash, parent
    public interface IDataStorage
    {
        /// <summary>
        /// Storage configuration.
        /// </summary>
        [NotNull] IDataStorageConfiguration Configuration { get; }

        /// <summary>
        /// Initializes DataStorage. Creates internal data structures, loads data.
        /// </summary>
        /// <returns>Initialize Task.</returns>
        Task Initialize();

        /// <summary>
        /// Gets entity list for entity type.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <returns>Entity list or null if not found.</returns>
        [CanBeNull] IEntityList GetEntityList([NotNull] Type entityType);

        /// <summary>
        /// Gets entity list.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <returns>Entity list.</returns>
        IEntityList<T> GetEntityList<T>() where T : class;
    }
}
