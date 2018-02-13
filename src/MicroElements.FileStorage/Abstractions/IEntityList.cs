// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using JetBrains.Annotations;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Marker interface for entity lists.
    /// </summary>
    public interface IEntityList { }

    /// <summary>
    /// Entity list.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public interface IEntityList<T> : IEntityList
    {
        /// <summary>
        /// Gets item by key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Item or null if not exist.</returns>
        [CanBeNull] T Get([NotNull] string key);

        /// <summary>
        /// Gets item by position.
        /// </summary>
        /// <param name="pos">Position, zero based row num.</param>
        /// <returns>Item or null if index is out of bound.</returns>
        [CanBeNull] T GetByPos(int pos);

        /// <summary>
        /// Gets a value indicating whether an entity exists.
        /// </summary>
        /// <param name="key">Entity key.</param>
        /// <returns>Returns <see langword="true"/> if entity exists.</returns>
        bool IsExists([NotNull] string key);

        /// <summary>
        /// Gets a value indicating whether collection is readonly.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets collection index.
        /// </summary>
        IIndex Index { get; }

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
}
