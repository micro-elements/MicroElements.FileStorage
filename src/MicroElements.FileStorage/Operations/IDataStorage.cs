// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.Operations
{
    public interface IDataStorage
    {
        /// <summary>
        /// Initializes DataStorage. Creates internal data structures, loads data.
        /// </summary>
        /// <returns>Initialize Task.</returns>
        Task Initialize();

        /// <summary>
        /// Gets entity list.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <returns>Entity list.</returns>
        IEntityList<T> GetEntityList<T>() where T : class;

        /// <summary>
        /// Gets all collections.
        /// </summary>
        /// <returns>Collection list.</returns>
        IReadOnlyList<Type> GetDocTypes();

        void Drop();
        void Save();

        //Name, Key, Hash, parent
    }
}
