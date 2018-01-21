// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentValidation;

namespace MicroElements.FileStorage.Validation
{
    /// <summary>
    /// Provides Null Object for <see cref="IValidatorFactory"/>.
    /// Assumes that all objects is valid.
    /// </summary>
    public class NullValidationFactory : IValidatorFactory
    {
        /// <inheritdoc />
        public IValidator<T> GetValidator<T>()
        {
            return NullValidator<T>.Instance;
        }

        /// <inheritdoc />
        public IValidator GetValidator(Type type)
        {
            return NullValidator<object>.Instance;
        }
    }
}
