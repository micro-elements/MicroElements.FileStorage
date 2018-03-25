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
        /// Gets count of items in collection.
        /// </summary>
        int Count { get; }
    }
}
