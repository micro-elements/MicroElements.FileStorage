// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace MicroElements.FileStorage.Operations
{
    public interface IDataStorage
    {
        Task Initialize();
    }
}
