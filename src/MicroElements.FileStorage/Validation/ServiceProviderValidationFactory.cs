// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentValidation;

namespace MicroElements.FileStorage.Validation
{
    /// <summary>
    /// <see cref="IValidatorFactory"/> implementation that uses <see cref="IServiceProvider"/> as source.
    /// </summary>
    public class ServiceProviderValidationFactory : IValidatorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderValidationFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public IValidator<T> GetValidator<T>()
        {
            return (IValidator<T>)_serviceProvider.GetService(typeof(IValidator<T>));
        }

        /// <inheritdoc />
        public IValidator GetValidator(Type type)
        {
            return (IValidator)_serviceProvider.GetService(typeof(IValidator<>).MakeGenericType(type));
        }
    }
}
