// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using JetBrains.Annotations;
using MicroElements.FileStorage.CodeContracts;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Entity and its key.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public struct EntityWithKey<T> where T : class
    {
        /// <summary>
        /// Entity.
        /// </summary>
        public T Entity;

        /// <summary>
        /// Entity key.
        /// </summary>
        public string Key;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityWithKey{T}"/> struct.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="key">Entity key.</param>
        public EntityWithKey([NotNull] T entity, [NotNull] string key)
        {
            Check.NotNull(entity, nameof(entity));
            Check.NotNull(key, nameof(key));

            Entity = entity;
            Key = key;
        }
    }
}
