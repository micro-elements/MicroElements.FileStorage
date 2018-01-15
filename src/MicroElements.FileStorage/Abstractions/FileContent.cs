// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MicroElements.FileStorage.CodeContracts;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Abstraction of file content.
    /// </summary>
    public class FileContent : IEquatable<FileContent>
    {
        /// <summary>
        /// File location.
        /// </summary>
        public string Location { get; }

        /// <summary>
        /// File content as string.
        /// </summary>
        public string Content { get; }

        //todo: BinaryContent, Encoding

        /// <summary>
        /// Initializes a new instance of the <see cref="FileContent"/> class.
        /// </summary>
        /// <param name="location">File location.</param>
        /// <param name="content">File content as string.</param>
        public FileContent([NotNull] string location, [NotNull] string content)
        {
            Check.NotNull(location, nameof(location));
            Check.NotNull(content, nameof(content));

            Location = location;
            Content = content;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FileContent);
        }

        public bool Equals(FileContent other)
        {
            return other != null &&
                   Location == other.Location &&
                   Content == other.Content;
        }

        public override int GetHashCode()
        {
            var hashCode = 319754776;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Location);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Content);
            return hashCode;
        }

        public static bool operator ==(FileContent content1, FileContent content2)
        {
            return EqualityComparer<FileContent>.Default.Equals(content1, content2);
        }

        public static bool operator !=(FileContent content1, FileContent content2)
        {
            return !(content1 == content2);
        }
    }
}
