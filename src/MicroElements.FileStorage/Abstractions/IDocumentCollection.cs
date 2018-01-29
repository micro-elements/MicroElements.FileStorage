// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

        /// <summary>
        /// Clears collection.
        /// </summary>
        void Drop();
    }
}
