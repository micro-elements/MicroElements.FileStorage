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
                await Executor.Execute(entityType, this, nameof(InitializeInternal), null);
            }
        }

        public async Task InitializeInternal<T>() where T : class
        {
            var entityList = _readOnlyDataStorage.GetEntityList<T>();

            if (entityList.Count > 0)
            {
                var list = (IEntityList<T>)GetOrCreateEntityList(typeof(T));
                entityList.ForEach((ent, key) => list.AddOrUpdate(ent, key));
            }
        }

        /// <inheritdoc />
        public IEntityList GetEntityList(Type entityType)
        {
            return GetOrCreateEntityList(entityType);
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
        public IDataStorageConfiguration Configuration => _configuration;
    }

    public static class Executor
    {
        public static Task Execute(Type genericParamType, object methodHost, string methodName, object arg = null)
        {
            var methodInfo = methodHost.GetType()
                .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .MakeGenericMethod(genericParamType);

            return (Task)methodInfo.Invoke(methodHost, arg != null ? new[] { arg } : null);
        }
    }
}
