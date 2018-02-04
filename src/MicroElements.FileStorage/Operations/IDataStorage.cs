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
        /// Get collection.
        /// </summary>
        /// <typeparam name="T">Document type.</typeparam>
        /// <returns>Typed IDocumentCollection.</returns>
        IDocumentCollection<T> GetCollection<T>() where T : class;

        IEntityList<T> GetDocList<T>() where T : class;

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
