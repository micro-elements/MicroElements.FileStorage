// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;

namespace MicroElements.FileStorage.Abstractions.Exceptions
{
    /// <summary>
    /// Base FileStorage exception.
    /// </summary>
    [Serializable]
    public class FileStorageException : Exception
    {
        public FileStorageException() { }

        public FileStorageException(string message) : base(message) { }

        public FileStorageException(string message, Exception innerException) : base(message, innerException) { }

        protected FileStorageException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
