// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Document collection with typed methods.
    /// </summary>
    /// <typeparam name="T">Type of document.</typeparam>
    public interface IDocumentCollection<T> : IDocumentCollection where T : class
    {
        /// <summary>
        /// Typed configuration.
        /// </summary>
        CollectionConfigurationTyped<T> ConfigurationTyped { get; }

        /// <summary>
        /// Adds new item to collection.//todo: database is readonly? addons!
        /// </summary>
        /// <param name="item">New item.</param>
        void Add([NotNull] T item);

        /// <summary>
        /// Gets item by key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Item or null if not exist.</returns>
        [CanBeNull] T Get([NotNull] string key); //todo: special struct for key

        /// <summary>
        /// Find all items matching the query.
        /// </summary>
        /// <param name="query">Filter predicate</param>
        /// <returns>Items matching the query</returns>
        IEnumerable<T> Find(Func<T, bool> query);

        /// <summary>
        /// Deletes entity by key.
        /// </summary>
        /// <param name="key">Entity key.</param>
        void Delete(string key);
    }
}
