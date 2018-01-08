// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// DataStore is document database.
    /// </summary>
    public interface IDataStore : IDisposable
    {
        /// <summary>
        /// Loads data.
        /// </summary>
        Task Initialize();

        /// <summary>
        /// Get collection.
        /// </summary>
        /// <typeparam name="T">Document type.</typeparam>
        /// <returns>Typed IDocumentCollection.</returns>
        IDocumentCollection<T> GetCollection<T>() where T : class;

        /// <summary>
        /// Gets all collections.
        /// </summary>
        /// <returns>Collection list.</returns>
        IReadOnlyList<IDocumentCollection> GetCollections();

        /// <summary>
        /// Saves changed collections.
        /// </summary>
        void Save();

        /// <summary>
        /// Drops all collections.
        /// </summary>
        void Drop();
    }
}