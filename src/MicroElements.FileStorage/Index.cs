// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MicroElements.FileStorage
{
    public class Index
    {
        // Key->index in list => поиск по ключу
        // Перечисление по индексу => итерация по списку
        // Удаления??? => 
        // Last EntityList со списком транзакций
        // Для сквозного поиска нужны все индексы + последний
        private readonly ConcurrentDictionary<string, int> _indexIdDocIndex = new ConcurrentDictionary<string, int>();
    }

    public interface IIndex<TBy>
    {
        IDictionary<TBy, int> Index();

        void Add(TBy item, int key);
    }
}
