// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.Serializers
{
    /// <summary>
    /// XmlSerializer is not production ready!!!.
    /// </summary>
    public class XmlSerializer : ISerializer
    {
        /// <inheritdoc />
        public IEnumerable<object> Deserialize(FileContent content, Type type)
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(type);
            var deserialize = xmlSerializer.Deserialize(new StringReader(content.Content));
            return (IEnumerable<object>)deserialize;
        }

        /// <inheritdoc />
        public IEnumerable<T> Deserialize<T>(FileContent content)
        {
            return (IEnumerable<T>)Deserialize(content, typeof(T));
        }

        /// <inheritdoc />
        public FileContent Serialize(IReadOnlyCollection<object> items, Type type)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public SerializerInfo GetInfo()
        {
            return new SerializerInfo
            {
                Extension = "xml"
            };
        }
    }
}
