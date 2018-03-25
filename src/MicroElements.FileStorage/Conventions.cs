// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.Serializers;

namespace MicroElements.FileStorage
{
    public class Conventions
    {
        public Func<ISerializer> GetDefaultSerializer { get; set; } = DefaultConventions.GetDefaultSerializer;
        public Func<ICollectionConfiguration, ISerializer> GetSerializer { get; set; } = DefaultConventions.DefaultResolveSerializer;

        public static readonly Conventions Default = new Conventions
        {
            GetDefaultSerializer = DefaultConventions.GetDefaultSerializer,
            GetSerializer = DefaultConventions.DefaultResolveSerializer
        };
    }

    internal static class DefaultConventions
    {
        public static ISerializer DefaultResolveSerializer(ICollectionConfiguration collectionConfiguration)
        {
            bool IsGoodFormat(string fmt) => !string.IsNullOrEmpty(fmt);

            var format = collectionConfiguration.Format;
            if (!IsGoodFormat(format) && collectionConfiguration.SourceFile != null)
                format = Path.GetExtension(collectionConfiguration.SourceFile);


            if (format == "json")
            {
                return new JsonSerializer();
            }

            if (format == "csv")
            {
                return new SimpleCsvSerializer();
            }

            return GetDefaultSerializer();
        }

        public static ISerializer GetDefaultSerializer()
        {
            return new JsonSerializer();
        }
    }
}
