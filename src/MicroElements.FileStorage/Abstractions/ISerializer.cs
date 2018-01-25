// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Entity serializer.
    /// </summary>
    [PublicAPI]
    public interface ISerializer
    {
        /// <summary>
        /// Deserializes entity from text content.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <param name="type">Entity type.</param>
        /// <returns>One entity or entity list.</returns>
        [NotNull] IEnumerable<object> Deserialize([NotNull] FileContent content, [NotNull] Type type);

        /// <summary>
        /// Serializes entity or entity list to <see cref="FileContent"/>.
        /// </summary>
        /// <param name="items">Entity or entity list.</param>
        /// <param name="type">Entity type.</param>
        /// <returns>FileContent</returns>
        [NotNull] FileContent Serialize([NotNull] IReadOnlyCollection<object> items, [NotNull] Type type);

        /// <summary>
        /// Gets serializer info.
        /// </summary>
        /// <returns>Serializer information.</returns>
        [NotNull] SerializerInfo GetInfo();
    }

    /// <summary>
    /// Serializer information.
    /// </summary>
    public class SerializerInfo
    {
        private string _extension = ".txt";

        /// <summary>
        /// File extension.
        /// </summary>
        public string Extension
        {
            get { return _extension; }
            set
            {
                if (string.IsNullOrEmpty(value) || !value.Contains("."))
                    throw new ArgumentException("Extension must be in format '.ext'");
                _extension = value;
            }
        }
    }

    /// <summary>
    /// Entity serializer.
    /// </summary>
    [PublicAPI]
    public interface IEntitySerializer
    {
        /// <summary>
        /// Serializes entity or entity list to <see cref="FileContent"/>.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="type">Entity type.</param>
        /// <returns>FileContent</returns>
        [NotNull] SerializedEntity Serialize([NotNull] object entity, [NotNull] Type type);

        /// <summary>
        /// Deserializes entity from text content.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <param name="type">Entity type.</param>
        /// <returns>One entity or entity list.</returns>
        [NotNull] object Deserialize([NotNull] SerializedEntity content, [NotNull] Type type);
    }

    public class SerializedEntity
    {
        public string Content { get; set; }
    }

    public class SerializerMetadata
    {
        public string Serializer { get; set; }
        public string Format { get; set; }// format is alternative for type
        public string Version { get; set; }// can be several versions or one format
    }
}
