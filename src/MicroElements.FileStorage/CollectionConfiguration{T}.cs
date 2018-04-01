// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentValidation;
using MicroElements.FileStorage.KeyAccessors;
using MicroElements.FileStorage.KeyGenerators;
using MicroElements.FileStorage.Validation;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Collection configuration for type.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class CollectionConfiguration<T> : CollectionConfiguration, ICollectionConfiguration<T> where T : class
    {
        public CollectionConfiguration()
        {
            DocumentType = typeof(T);
        }

        public CollectionConfiguration(ICollectionConfiguration configuration)
        {
            if (configuration.DocumentType != typeof(T))
                throw new InvalidOperationException($"Inconsistent entity type. Expected {typeof(T)} but was {configuration.DocumentType}");

            DocumentType = configuration.DocumentType;
            Name = configuration.Name;
            Format = configuration.Format;
            Serializer = configuration.Serializer;
            SourceFile = configuration.SourceFile;
            Version = configuration.Version;
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
