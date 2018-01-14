// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using JetBrains.Annotations;
using MicroElements.FileStorage.CodeContracts;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Abstraction of file content.
    /// </summary>
    public class FileContent
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
    }

    //todo: design. move
    public class FileContentMetadata
    {
        public DateTime CreationTimeUtc { get; }
        public DateTime LastWriteTimeUtc { get; }
        public DateTime DateModified { get; }
        public bool IsExists { get; }
        public bool IsReadonly { get; }
        public long Length { get; }
    }

    // todo: unify location related stuff in Location class
    public class Location
    {
        private Uri _uri;//todo: see Spring.Net and cake build alternatives
    }
}
