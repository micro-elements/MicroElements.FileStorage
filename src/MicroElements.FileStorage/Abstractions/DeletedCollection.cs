using System.Collections.Generic;
using System.Linq;
using ConcurrentCollections;

namespace MicroElements.FileStorage.Abstractions
{
    public abstract class DeletedCollection
    {
        protected readonly ConcurrentHashSet<string> KeysForDelete = new ConcurrentHashSet<string>();
        
        protected internal IEnumerable<string> GetDeletedKey()
        {
            return KeysForDelete.AsEnumerable();
        }

        protected internal void DeleteKey(string key)
        {
            KeysForDelete.TryRemove(key);
        }
    }
}
