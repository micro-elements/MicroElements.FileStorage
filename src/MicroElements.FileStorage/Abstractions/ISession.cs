// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Unit of work for data store change.
    /// <para>All changes will be saved on dispose or <see cref="SaveChanges"/> call.</para>
    /// </summary>
    public interface ISession : IDisposable
    {
        /// <summary>
        /// Adds entity or updates if entity already exists.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <param name="entity">Entity.</param>
        /// <param name="key">Optional key. If no key provided then it will be get from entity or generated according rules.</param>
        void AddOrUpdate<T>([NotNull] T entity, [CanBeNull] string key = null) where T : class;

        /// <summary>
        /// Deletes entity.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <param name="key">Entity key.</param>
        void Delete<T>([NotNull] string key) where T : class;

        /// <summary>
        /// Patches or overrides some properties of an existing entity.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <param name="key">Entity key.</param>
        /// <param name="properties">Properties.</param>
        void Patch<T>([NotNull] string key, IDictionary<string, object> properties) where T : class;

        /// <summary>
        /// Deletes all entities of type.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        void Drop<T>() where T : class;

        /// <summary>
        /// Saves changes to <see cref="IStorageProvider"/>.
        /// <para>Can be called several times if needed.</para>
        /// <para>Calls automatically on session dispose.</para>
        /// </summary>
        void SaveChanges();
    }
}
