// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;
using FluentValidation.Results;

namespace MicroElements.FileStorage.Validation
{
    /// <summary>
    /// Null validator is always valid implementation.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class NullValidator<T> : AbstractValidator<T>
    {
        public static readonly NullValidator<T> Instance = new NullValidator<T>();

        /// <inheritdoc />
        public override ValidationResult Validate(ValidationContext<T> context)
        {
            return new ValidationResult();
        }
    }
}
