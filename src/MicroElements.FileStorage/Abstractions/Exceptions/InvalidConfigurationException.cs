// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;

namespace MicroElements.FileStorage.Abstractions.Exceptions
{
    [Serializable]
    public class InvalidConfigurationException : FileStorageException
    {
        public InvalidConfigurationException() { }

        public InvalidConfigurationException(string message) : base(message) { }

        public InvalidConfigurationException(string message, Exception innerException) : base(message, innerException) { }

        protected InvalidConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
