// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;

namespace MicroElements.FileStorage
{
    public class DataStorageConfigurationValidator : AbstractValidator<DataStorageConfiguration>
    {
        public DataStorageConfigurationValidator()
        {
            RuleFor(configuration => configuration.StorageProvider).NotNull();
        }
    }
}
