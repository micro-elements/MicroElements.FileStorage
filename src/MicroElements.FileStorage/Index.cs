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

        //todo: set<string>
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
}
