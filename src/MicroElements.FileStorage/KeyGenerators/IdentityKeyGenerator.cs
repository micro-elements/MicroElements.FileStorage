using System;
using System.Linq;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.KeyGenerators
{
    public class IdentityKeyGenerator<T> : IKeyGenerator<T> where T : class
    {
        private readonly int _startValue;
        private readonly bool _useCollectionPrefix;

        /// <inheritdoc />
        public IdentityKeyGenerator(int startValue = 0, bool useCollectionPrefix = true)
        {
            _startValue = startValue;
            _useCollectionPrefix = useCollectionPrefix;
        }

        /// <inheritdoc />
        public KeyType KeyStrategy { get; } = KeyType.Identity;

        /// <inheritdoc />
        public Key GetNextKey(IDocumentCollection<T> collection)
        {
            string collectionName = collection.ConfigurationTyped.Name;

            int ParseKey(string key)
            {
                if (key.Length > collectionName.Length + 1 && key.StartsWith(collectionName))
                {
                    key = key.Substring(collectionName.Length + 1);
                }
                return Int32.Parse(key);
            }

            int max = _startValue;
            if (collection.Count > 0)
            {
                max = collection
                    .Find(arg => true)
                    .Select(arg => collection.ConfigurationTyped.KeyGetter.GetIdFunc()(arg))
                    .Select(ParseKey)
                    .Max();
            }
            int nextId = max + 1;

            return new Key(KeyType.Identity, nextId.ToString(), collectionName);
        }


    }
}