// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using MicroElements.FileStorage.Abstractions;

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

        IEnumerable<string> AddedKeys { get; }

        //todo: set<string> or IsDeleted
        IEnumerable<string> DeletedKeys { get; }
    }

    public class Index : IIndex
    {
        /// <inheritdoc />
        public Index(IReadOnlyDictionary<string, int> keyPosition, IEnumerable<string> addedKeys, IEnumerable<string> deletedKeys)
        {
            KeyPosition = keyPosition;
            AddedKeys = addedKeys;
            DeletedKeys = deletedKeys;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, int> KeyPosition { get; }

        /// <inheritdoc />
        public IEnumerable<string> AddedKeys { get; }

        /// <inheritdoc />
        public IEnumerable<string> DeletedKeys { get; }
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
                foreach (var deletedKey in indices[i].DeletedKeys)
                {
                    fullIndex.Remove(deletedKey);
                }
            }

            return fullIndex;
        }
    }
}
