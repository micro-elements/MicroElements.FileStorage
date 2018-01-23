// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentValidation;
using MicroElements.FileStorage.Abstractions.Exceptions;
using MicroElements.FileStorage.KeyAccessors;
using MicroElements.FileStorage.KeyGenerators;
using MicroElements.FileStorage.Serializers;
using MicroElements.FileStorage.Validation;

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

        /// <summary>
        /// Verifies correctness of configuration.
        /// Throws <see cref="InvalidConfigurationException"/> if configuration is not valid.
        /// </summary>
        public virtual void Verify()
        {
            if (SourceFile == null)
                throw new InvalidConfigurationException("SourceFile is required.");

            if (Serializer == null)
                Serializer = new JsonSerializer();
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

        public IValidatorFactory ValidatorFactory { get; set; } = new NullValidationFactory();

        /// <inheritdoc />
        public override void Verify()
        {
            base.Verify();

            // Setting default implementations.
            KeyGetter = KeyGetter ?? DefaultKeyAccessor<T>.Instance;
            KeySetter = KeySetter ?? DefaultKeyAccessor<T>.Instance;
            KeyGenerator = KeyGenerator ?? new GuidKeyGenerator<T>();
            ValidatorFactory = ValidatorFactory ?? new NullValidationFactory();
        }
    }
}
