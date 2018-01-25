// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using ConcurrentCollections;

namespace MicroElements.FileStorage.Abstractions
{
    public abstract class DeletedCollection
    {
        private readonly ConcurrentHashSet<string> _keysForDelete = new ConcurrentHashSet<string>();

        protected ConcurrentHashSet<string> KeysForDelete => _keysForDelete;

        protected internal IEnumerable<string> GetDeletedKey()
        {
            return _keysForDelete.AsEnumerable();
        }

        protected internal void DeleteKey(string key)
        {
            _keysForDelete.TryRemove(key);
        }
    }
}
