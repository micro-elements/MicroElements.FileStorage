// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.Operations;

namespace MicroElements.FileStorage
{
    public class CommandLogDocCollection<T> : IEntityList<T> where T : class
    {
        private ICommandLog _commandLog = new CommandLog();

        /// <inheritdoc />
        public T Get(string key)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool IsExists(string key)
        {
            return false;
        }

        /// <inheritdoc />
        public void Freeze()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool IsReadOnly { get; }

        /// <inheritdoc />
        public void AddOrUpdate(T item, string key)
        {
            _commandLog.Add(new StoreCommand(CommandType.Store, typeof(T), key) { Entity = item });
        }

        /// <inheritdoc />
        public void Delete(string key)
        {
            _commandLog.Add(new StoreCommand(CommandType.Delete, typeof(T), key));
        }

        /// <inheritdoc />
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IIndex Index { get; }

        /// <inheritdoc />
        public T GetByPos(int pos)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Initialize(IReadOnlyList<EntityWithKey<T>> added, IReadOnlyList<string> deleted)
        {
            throw new NotImplementedException();
        }
    }
}
