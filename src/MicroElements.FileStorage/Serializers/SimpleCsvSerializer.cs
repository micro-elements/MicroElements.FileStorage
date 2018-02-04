// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using MicroElements.FileStorage.Abstractions;
using Newtonsoft.Json;

namespace MicroElements.FileStorage.Serializers
{
    /// <summary>
    /// Simple CSV serializer.
    /// Assumes csv with header row and comma as separator.
    /// Converts each line to json and uses Json.Net to deserialize object.
    /// </summary>
    public class SimpleCsvSerializer : ISerializer
    {
        private readonly char _separator = ',';

        /// <inheritdoc />
        public IEnumerable<object> Deserialize(FileContent content, Type type)
        {
            var lines = content.Content.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var headers = lines[0].Split(_separator);
            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(_separator);
                StringBuilder json = new StringBuilder();
                json.Append("{");
                for (int h = 0; h < headers.Length; h++)
                {
                    var property = headers[h];
                    var value = values[h];
                    json.AppendFormat($@"""{property}"": ""{value}""");
                    if (h < headers.Length - 1)
                        json.Append(",");
                }
                json.Append("}");

                var deserialized = JsonConvert.DeserializeObject(json.ToString(), type);
                yield return deserialized;
            }
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
                Extension = ".csv"
            };
        }
    }
}
