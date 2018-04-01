// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Generic collection configuration.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public interface ICollectionConfiguration<T> : ICollectionConfiguration where T : class
    {
        /// <summary>
        /// KeyGetter for type. If key is null than <see cref="KeyGenerator"/> uses for key generation and <see cref="KeySetter"/> for setting key.
        /// </summary>
        IKeyGetter<T> KeyGetter { get; }

        /// <summary>
        /// KeySetter for type.
        /// </summary>
        IKeySetter<T> KeySetter { get; }

        /// <summary>
        /// Key generator for entities with null key.
        /// </summary>
        IKeyGenerator<T> KeyGenerator { get; }

        /// <summary>
        /// ValidatorFactory for entity validation.
        /// </summary>
        IValidatorFactory ValidatorFactory { get; }
    }
}
