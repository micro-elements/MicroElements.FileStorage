// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace MicroElements.FileStorage.Validation
{
    public class SimpleValidationFactory : IValidatorFactory
    {
        private IValidator[] _validators;
        private Dictionary<Type, IValidator> _validators2 = new Dictionary<Type, IValidator>();

        // todo: doesnot work if registered as IValidator<Entity>
        public SimpleValidationFactory(IEnumerable<IValidator> validators)
        {
            _validators = validators.ToArray();
        }

        /// <inheritdoc />
        public IValidator<T> GetValidator<T>()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IValidator GetValidator(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
