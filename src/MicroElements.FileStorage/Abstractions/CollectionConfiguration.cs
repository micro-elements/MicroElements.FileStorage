using System;
using MicroElements.FileStorage.KeyGenerators;
using MicroElements.FileStorage.Serializers;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Collection configuration.
    /// todo: make readonly or freezable
    /// </summary>
    public class CollectionConfiguration
    {
        private Type _documentType;

        /// <summary>
        /// Name of collection.
        /// <para>Also uses for key prefixes.</para>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Document type.
        /// </summary>
        public Type DocumentType
        {
            get { return _documentType; }
            set
            {
                _documentType = value;
                if (Name == null && _documentType != null)
                {
                    Name = _documentType.Name.ToLowerInvariant();
                }
            }
        }

        public string SourceFile { get; set; }
        public string Format { get; set; }

        /// <summary>
        /// Serializer for document content.
        /// <para>If not set then <see cref="JsonSerializer"/> uses.</para>
        /// </summary>
        public ISerializer Serializer { get; set; } = new JsonSerializer();

        // not used yet
        public bool OneFilePerCollection { get; set; }
        public string Version { get; set; }

        //todo: verify correctness.
        public void Verify()
        {

        }
    }

    public class CollectionConfigurationTyped<T> : CollectionConfiguration where T : class
    {
        /// <inheritdoc />
        public CollectionConfigurationTyped()
        {
            DocumentType = typeof(T);
        }

        public IKeyGetter<T> KeyGetter { get; set; } = DefaultKeyAccessor<T>.Instance;
        public IKeySetter<T> KeySetter { get; set; } = DefaultKeyAccessor<T>.Instance;
        public IKeyGenerator<T> KeyGenerator { get; set; } = new GuidKeyGenerator<T>();
    }
}