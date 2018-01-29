// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using ConcurrentCollections;

namespace MicroElements.FileStorage
{
    public class DelayedOperations
    {
        private readonly ConcurrentHashSet<string> _keysForDelete = new ConcurrentHashSet<string>();

        public void MarkAsDeleted(string key)
        {
            _keysForDelete.Add(key);
        }

        public IEnumerable<string> GetDeletedKeys()
        {
            return _keysForDelete.AsEnumerable();
        }

        public void RemoveKeyFromDeleteList(string key)
        {
            _keysForDelete.TryRemove(key);
        }
    }
}
