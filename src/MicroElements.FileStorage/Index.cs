// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.CodeContracts;

namespace MicroElements.FileStorage
{
    // Key->index in list => поиск по ключу
    // Перечисление по индексу => итерация по списку
    // Удаления??? => 
    // Last EntityList со списком транзакций
    // Для сквозного поиска нужны все индексы + последний

    public interface IIndex
    {
        IReadOnlyDictionary<string, int> KeyPosition { get; }

        ISet<string> AddedKeys { get; }

        ISet<string> DeletedKeys { get; }
    }

    public class Index : IIndex
    {
        public Index(IReadOnlyDictionary<string, int> keyPosition, ISet<string> addedKeys, ISet<string> deletedKeys)
        {
            Check.NotNull(keyPosition, nameof(keyPosition));
            Check.NotNull(addedKeys, nameof(addedKeys));
            Check.NotNull(deletedKeys, nameof(deletedKeys));
            if (addedKeys.Count + deletedKeys.Count != keyPosition.Count)
                throw new ArgumentException("addedKeys.Count + deletedKeys.Count != keyPosition.Count");

            KeyPosition = keyPosition;
            AddedKeys = addedKeys;
            DeletedKeys = deletedKeys;
        }

        public Index(IReadOnlyDictionary<string, int> keyPosition, ISet<string> deletedKeys)
        {
            Check.NotNull(keyPosition, nameof(keyPosition));
            Check.NotNull(deletedKeys, nameof(deletedKeys));

            KeyPosition = keyPosition;
            var added = new HashSet<string>(keyPosition.Keys);
            added.ExceptWith(deletedKeys);
            AddedKeys = added;
            DeletedKeys = deletedKeys;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, int> KeyPosition { get; }

        /// <inheritdoc />
        public ISet<string> AddedKeys { get; }

        /// <inheritdoc />
        public ISet<string> DeletedKeys { get; }
    }

    public interface IIndex<TBy>
    {
        IReadOnlyDictionary<TBy, int> Index();
    }

    public class IndexKey<T>
    {
        public string Key { get; }
        public IEntityList<T> EntityList { get; }
        public int Pos { get; }

        public IndexKey(string key, IEntityList<T> entityList, int pos)
        {
            Key = key;
            EntityList = entityList;
            Pos = pos;
        }
    }

    public static class IndexBuilder
    {
        public static IDictionary<string, IndexKey<T>> BuildFullIndex<T>(IReadOnlyList<IIndex> indices, IReadOnlyList<IEntityList<T>> entityLists) where T : class
        {
            Dictionary<string, IndexKey<T>> fullIndex = new Dictionary<string, IndexKey<T>>();
            for (int i = 0; i < indices.Count; i++)
            {
                foreach (var valuePair in indices[i].KeyPosition)
                {
                    fullIndex.Add(valuePair.Key, new IndexKey<T>(valuePair.Key, entityLists[i], valuePair.Value));
                }

                if (indices[i].DeletedKeys != null)
                {
                    foreach (var deletedKey in indices[i].DeletedKeys)
                    {
                        fullIndex.Remove(deletedKey);
                    }
                }
            }

            return fullIndex;
        }
    }
}
