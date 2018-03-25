// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroElements.FileStorage.Operations;

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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// todo: split on init and load
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

        DataStoreConfiguration Configuration { get; }

        IReadOnlyList<IDataStorage> Storages { get; }

        Schema Schema { get; }

        DataStoreServices Services { get; }

        ISession OpenSession();
    }
}
