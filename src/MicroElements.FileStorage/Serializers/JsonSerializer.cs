using System;
using System.Collections.Generic;
using MicroElements.FileStorage.Abstractions;
using Newtonsoft.Json;

namespace MicroElements.FileStorage.Serializers
{
    /// <summary>
    /// Json.Net serializer.
    /// </summary>
    public class JsonSerializer : ISerializer
    {
        /// <inheritdoc />
        public IEnumerable<object> Deserialize(FileContent content, Type type)
        {
            string text = content.Content;
            var isList = text.StartsWith("[");
            if (isList)
            {
                Type listType = typeof(List<>).MakeGenericType(type);
                var deserializedList = JsonConvert.DeserializeObject(text, listType);
                return (IEnumerable<object>)deserializedList;
            }

            var deserialized = JsonConvert.DeserializeObject(text, type);
            return deserialized != null ? new[] { deserialized } : Array.Empty<object>();
        }

        /// <inheritdoc />
        public FileContent Serialize(IReadOnlyCollection<object> items, Type type)
        {
            var serialized = JsonConvert.SerializeObject(items);
            return new FileContent(String.Empty, serialized);
        }
    }
}