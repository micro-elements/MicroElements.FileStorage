// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.FileStorage.Abstractions
{
    public class StorageMetadata
    {
        /// <summary>
        /// Storage is read only.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /*
        // todo: StorageMetadata:
            Name
            FullSize?
            IsReadonly
            FullInMemory | LazyLoading
            Remote?
            Async?
            TimeOut
        */
    }
}
