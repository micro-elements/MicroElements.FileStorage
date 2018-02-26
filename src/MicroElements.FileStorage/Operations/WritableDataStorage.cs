// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.Operations
{
    public class WritableDataStorage : IWritableDataStorage
    {
        private readonly IDataStore _dataStore;
        private readonly DataStorageConfiguration _configuration;
        private readonly ReadOnlyDataStorage _readOnlyDataStorage;
        private List<StoreCommand> _commands = new List<StoreCommand>();
        private ConcurrentDictionary<Type, IEntityList> _entityLists = new ConcurrentDictionary<Type, IEntityList>();


        public WritableDataStorage(IDataStore dataStore, DataStorageConfiguration configuration)
        {
            _dataStore = dataStore;
            _configuration = configuration;
            _readOnlyDataStorage = new ReadOnlyDataStorage(dataStore, configuration);
        }

        /// <inheritdoc />
        public void Add(StoreCommand command)
        {
            var addInternal = GetType().GetMethod(nameof(AddInternal), BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(StoreCommand) }, null);
            addInternal = addInternal.MakeGenericMethod(command.EntityType);
            addInternal.Invoke(this, new[] { command });
        }

        private void AddInternal<T>(StoreCommand command) where T : class
        {
            _commands.Add(command);

            var entityList = GetEntityList<T>();

            if (command.CommandType == CommandType.Store)
            {
                entityList.AddOrUpdate((T)command.Entity, command.Key);
            }
            if (command.CommandType == CommandType.Delete)
            {
                entityList.Delete(command.Key);
            }
        }

        /// <inheritdoc />
        public async Task Initialize()
        {
            await _readOnlyDataStorage.Initialize();
            var entityTypes = _readOnlyDataStorage.Configuration.Collections.Select(configuration => configuration.DocumentType);
            foreach (var entityType in entityTypes)
            {
                Executor.SaveCollection(() => entityType, this);
                var entityList = _readOnlyDataStorage.GetEntityList<object>();
                GetOrCreateEntityList(entityType).AddAll(entityList);
            }
        }

        /// <inheritdoc />
        public IDocumentCollection<T> GetCollection<T>() where T : class
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IEntityList<T> GetEntityList<T>() where T : class
        {
            return (IEntityList<T>)GetOrCreateEntityList(typeof(T));
        }

        private IEntityList GetOrCreateEntityList(Type entityType)
        {
            return _entityLists.GetOrAdd(entityType, type => EntityListFactory.Create(typeof(WritableEntityList<>), entityType));
        }

        /// <inheritdoc />
        public IReadOnlyList<Type> GetDocTypes()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IDataStorageConfiguration Configuration => _configuration;

        /// <inheritdoc />
        public IReadOnlyList<IDocumentCollection> GetCollections()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Drop()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Save()
        {
            throw new NotImplementedException();
        }
    }

    public static class Executor
    {
        public static Task SaveCollection(Func<Type> getType, object arg)
        {
            var methodInfo = typeof(Executor)
                .GetMethod(nameof(SaveCollectionInternal), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(getType());

            return (Task)methodInfo.Invoke(null, new[] { arg });
        }

        private static async Task SaveCollectionInternal<T>(object arg)
        {

        }
    }
}
