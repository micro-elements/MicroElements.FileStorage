// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.CodeContracts;
using Newtonsoft.Json;

namespace MicroElements.FileStorage.Serializers
{
    /// <summary>
    /// Json.Net serializer.
    /// </summary>
    public class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer"/> class.
        /// </summary>
        /// <param name="jsonSerializerSettings"><see cref="JsonSerializerSettings"/>.</param>
        public JsonSerializer(JsonSerializerSettings jsonSerializerSettings = null)
        {
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        /// <inheritdoc />
        public IEnumerable<object> Deserialize(FileContent content, Type type)
        {
            Check.NotNull(content, nameof(content));
            Check.NotNull(type, nameof(type));

            string text = content.Content;
            var isList = text.StartsWith("[");
            if (isList)
            {
                Type listType = typeof(List<>).MakeGenericType(type);
                var deserializedList = JsonConvert.DeserializeObject(text, listType, _jsonSerializerSettings);
                return (IEnumerable<object>)deserializedList;
            }

            var deserialized = JsonConvert.DeserializeObject(text, type, _jsonSerializerSettings);
            return deserialized != null ? new[] { deserialized } : Array.Empty<object>();
        }

        /// <inheritdoc />
        public IEnumerable<T> Deserialize<T>(FileContent content)
        {
            Check.NotNull(content, nameof(content));

            string text = content.Content;
            var isList = text.StartsWith("[");
            if (isList)
            {
                Type listType = typeof(List<>).MakeGenericType(typeof(T));
                var deserializedList = (List<T>)JsonConvert.DeserializeObject(text, listType, _jsonSerializerSettings);
                return deserializedList;
            }

            var deserialized = (T)JsonConvert.DeserializeObject(text, typeof(T), _jsonSerializerSettings);
            return deserialized != null ? new[] { deserialized } : Array.Empty<T>();
        }

        /// <inheritdoc />
        public FileContent Serialize(IReadOnlyCollection<object> items, Type type)
        {
            Check.NotNull(items, nameof(items));
            Check.NotNull(type, nameof(type));

            string serialized = "(empty)";
            if (items.Count > 1)
            {
                serialized = JsonConvert.SerializeObject(items, _jsonSerializerSettings);
            }
            else if (items.Count == 1)
            {
                serialized = JsonConvert.SerializeObject(items.First(), _jsonSerializerSettings);
            }

            return new FileContent(string.Empty, serialized);
        }

        /// <inheritdoc />
        public SerializerInfo GetInfo()
        {
            return new SerializerInfo
            {
                Extension = ".json"
            };
        }
    }
}
