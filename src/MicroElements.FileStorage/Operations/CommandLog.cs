// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DynamicData;

namespace MicroElements.FileStorage.Operations
{
    public class CommandLog : ICommandLog
    {
        private readonly ISourceCache<StoreCommand, string> _commands = new SourceCache<StoreCommand, string>(command => command.Key);

        public void Add(StoreCommand command)
        {
            _commands.AddOrUpdate(command);
        }

        public void BulkAdd(StoreCommand[] commands)
        {
            foreach (var storeCommand in commands)
            {
                _commands.AddOrUpdate(storeCommand);
            }
        }

        //public object this[int index] => _commands[index];

        public int Count => _commands.Count;

        /// <inheritdoc />
        public IEnumerator<StoreCommand> GetEnumerator() => _commands.Items.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
