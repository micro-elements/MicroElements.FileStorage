// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MicroElements.FileStorage.Operations
{
    public class CommandLog : ICommandLog
    {
        //private readonly List<StoreCommand> _commands = new List<StoreCommand>();
        //private BlockingCollection<StoreCommand> _blockingCollection = new BlockingCollection<StoreCommand>();
        private ConcurrentQueue<StoreCommand> _commands = new ConcurrentQueue<StoreCommand>();

        public void Add(StoreCommand command)
        {
            // todo: what about double delete?
            _commands.Enqueue(command);
        }

        public void BulkAdd(StoreCommand[] commands)
        {
            foreach (var storeCommand in commands)
            {
                _commands.Enqueue(storeCommand);
            }
        }

        //public object this[int index] => _commands[index];

        public int Count => _commands.Count;

        /// <inheritdoc />
        public IEnumerator<StoreCommand> GetEnumerator() => _commands.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
