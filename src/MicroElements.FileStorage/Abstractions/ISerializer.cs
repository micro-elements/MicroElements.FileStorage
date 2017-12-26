using System;
using System.Collections.Generic;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Entity serializer.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Deserializes entity from text content.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <param name="type">Entity type.</param>
        /// <returns>One entity or entity list.</returns>
        IEnumerable<object> Deserialize(FileContent content, Type type);

        /// <summary>
        /// Serializes entity or entity list to <see cref="FileContent"/>.
        /// </summary>
        /// <param name="items">Entity or entity list.</param>
        /// <param name="type">Entity type.</param>
        /// <returns>FileContent</returns>
        FileContent Serialize(IReadOnlyCollection<object> items, Type type);
    }
}