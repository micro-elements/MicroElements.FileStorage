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
        ICollectionConfiguration<T> ConfigurationTyped { get; }

        /// <summary>
        /// Adds new item to collection.
        /// </summary>
        /// <param name="item">New item.</param>
        void AddOrUpdate([NotNull] T item);

        /// <summary>
        /// Gets item by key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Item or null if not exist.</returns>
        [CanBeNull] T Get([NotNull] string key);

        /// <summary>
        /// Gets a value indicating whether an entity exists.
        /// </summary>
        /// <param name="key">Entity key.</param>
        /// <returns>Returns <see langword="true"/> if entity exists.</returns>
        bool IsExists([NotNull] string key);

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
        void Delete([NotNull] string key);
    }
}
