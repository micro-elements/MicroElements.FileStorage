// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using MicroElements.FileStorage.Abstractions;

/*
This file contains drafts and ideas.
Can be deleted at any time or moved to right place.
*/

namespace MicroElements.FileStorage.Experimental
{
    internal interface IDataConvertor { }


    internal interface IDocumentCollectionFactory
    {
        CollectionConfiguration Get(Type entityType);
    }

    internal class DocumentCollectionFactory : IDocumentCollectionFactory
    {
        private readonly IReadOnlyList<CollectionConfiguration> _configurations;

        /// <inheritdoc />
        public DocumentCollectionFactory(IEnumerable<CollectionConfiguration> configurations)
        {
            _configurations = new List<CollectionConfiguration>(configurations);
        }

        public CollectionConfiguration Get(Type entityType)
        {
            return _configurations.FirstOrDefault(configuration => configuration.DocumentType == entityType);
        }
    }

    internal class CollectionKey
    {
        public Type Type { get; }
        public string Name { get; }

        /// <inheritdoc />
        public CollectionKey(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        /// <inheritdoc />
        public CollectionKey(Type type)
        {
            Type = type;
            Name = type.FullName;
        }
    }

}
