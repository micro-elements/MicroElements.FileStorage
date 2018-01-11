// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Document collection.
    /// </summary>
    public interface IDocumentCollection
    {
        /// <summary>
        /// Configuration.
        /// </summary>
        CollectionConfiguration Configuration { get; }

        /// <summary>
        /// Collection has changes.
        /// </summary>
        bool HasChanges { get; set; }

        /// <summary>
        /// Gets count of items in collection.
        /// </summary>
        int Count { get; }

        [Obsolete]
        IReadOnlyCollection<object> GetAll();

        void Drop();
    }

    /// <summary>
    /// Document collection with typed methods.
    /// </summary>
    /// <typeparam name="T">Type of document.</typeparam>
    public interface IDocumentCollection<T> : IDocumentCollection where T : class
    {
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
    }



    public interface IDocumentCollectionFactory
    {
        CollectionConfiguration Get(Type entityType);
    }

    public class DocumentCollectionFactory : IDocumentCollectionFactory
    {
        private readonly IReadOnlyList<CollectionConfiguration> _configurations;

        /// <inheritdoc />
        public DocumentCollectionFactory(IEnumerable<CollectionConfiguration> configurations)
        {
            _configurations = new List<CollectionConfiguration>(configurations);
        }

        public CollectionConfiguration Get(Type entityType)
        {
            return _configurations.FirstOrDefault(configuration => configuration.DocumentType == entityType);
        }
    }

    public class CollectionKey
    {
        public Type Type { get; }
        public string Name { get; }

        /// <inheritdoc />
        public CollectionKey(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        /// <inheritdoc />
        public CollectionKey(Type type)
        {
            Type = type;
            Name = type.FullName;
        }
    }
}
