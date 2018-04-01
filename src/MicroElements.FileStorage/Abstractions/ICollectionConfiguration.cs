// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MicroElements.FileStorage.Abstractions.Exceptions;
using MicroElements.FileStorage.Serializers;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Collection configuration.
    /// </summary>
    public interface ICollectionConfiguration
    {
        /// <summary>
        /// Document type.
        /// </summary>
        Type DocumentType { get; }

        /// <summary>
        /// Serializer for document content.
        /// <para>If not set then <see cref="JsonSerializer"/> uses.</para>
        /// </summary>
        ISerializer Serializer { get; }

        /// <summary>
        /// Name of collection.
        /// <para>Also uses for key prefixes.</para>
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Source file or directory.
        /// </summary>
        string SourceFile { get; }

        /// <summary>
        /// Format.
        /// </summary>
        string Format { get; }

        /// <summary>
        /// Version. Uses with Format.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Verifies correctness of configuration.
        /// Throws <see cref="InvalidConfigurationException"/> if configuration is not valid.
        /// </summary>
        /// todo: remove verify or move to other layer
        void Verify();
    }
}
