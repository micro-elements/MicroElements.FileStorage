using System;
using System.IO;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.Serializers;

namespace MicroElements.FileStorage
{
    public class Conventions
    {
        public Func<ISerializer> GetDefaultSerializer = DefaultConventions.GetDefaultSerializer;
        public Func<CollectionConfiguration, ISerializer> GetSerializer = DefaultConventions.DefaultResolveSerializer;

        public static readonly Conventions Default = new Conventions
        {
            GetDefaultSerializer = DefaultConventions.GetDefaultSerializer,
            GetSerializer = DefaultConventions.DefaultResolveSerializer
        };
    }

    internal static class DefaultConventions
    {
        public static ISerializer DefaultResolveSerializer(CollectionConfiguration collectionConfiguration)
        {
            bool IsGoodFormat(string fmt) => !String.IsNullOrEmpty(fmt);

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