// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace MicroElements.FileStorage.Abstractions
{
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
}
