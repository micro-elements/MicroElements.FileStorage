// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DynamicData;
using JetBrains.Annotations;

namespace MicroElements.FileStorage.Abstractions
{
    public interface IReadOnlyDocumentContainer<T>
    {
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

        //IndexState GetIndexState();
    }

    public enum IndexState
    {
        NotFound,
        Exists,
        Deleted
    }

    /// <summary>
    /// Entity list.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public interface IDocumentContainer<T> : IReadOnlyDocumentContainer<T>
    {
        bool IsReadOnly { get; }

        /// <summary>
        /// Adds new item to collection.
        /// </summary>
        /// <param name="item">Entity.</param>
        /// <param name="key">Entity key.</param>
        void AddOrUpdate([NotNull] T item, [NotNull] string key);

        /// <summary>
        /// Deletes entity by key.
        /// </summary>
        /// <param name="key">Entity key.</param>
        void Delete([NotNull] string key);
    }

    public class ChainNode<T>
    {
        public T Value { get; set; }
        public T Parent { get; set; }
    }

    public class ContainerChain<T> : IDocumentContainer<T>
    {
        private ImmutableArray<IDocumentContainer<T>> _containers;

        /// <inheritdoc />
        public ContainerChain(IDocumentContainer<T> container)
        {
            _containers = _containers.Add(container);
        }

        public void AddContainer(IDocumentContainer<T> container)
        {
            _containers = _containers.Add(container);
        }

        private IDocumentContainer<T> GetLast()
        {
            return _containers.Last();
        }


        /// <inheritdoc />
        public T Get(string key)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public bool IsExists(string key)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public bool IsReadOnly { get; }

        /// <inheritdoc />
        public void AddOrUpdate(T item, string key)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void Delete(string key)
        {
            throw new System.NotImplementedException();
        }
    }


}
