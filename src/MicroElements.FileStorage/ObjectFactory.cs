// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage
{
    public static class ObjectFactory
    {
        public static IEntityList Create(Type entityListType, CollectionData collectionData)
        {
            var genericType = entityListType.MakeGenericType(collectionData.EntityType);
            return (IEntityList)Activator.CreateInstance(genericType, collectionData);
        }

        public static IEntityList Create(Type entityListType, Type entityType)
        {
            var genericType = entityListType.MakeGenericType(entityType);
            return (IEntityList)Activator.CreateInstance(genericType);
        }

        public static IEntityList Create(Type entityListType, Type entityType, object arg)
        {
            var genericType = entityListType.MakeGenericType(entityType);
            return (IEntityList)Activator.CreateInstance(genericType, arg);
        }

        public static IDocumentCollection CreateDocumentCollection(Type entityListType, Type entityType, object arg)
        {
            var genericType = entityListType.MakeGenericType(entityType);
            return (IDocumentCollection)Activator.CreateInstance(genericType, arg);
        }
    }
}
