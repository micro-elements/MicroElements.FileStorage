// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.FileStorage.Operations
{
    public interface IDataAddon : IDataStorage
    {
        void Add(StoreCommand command);
    }

    public interface IAddonManager
    {
        /// <summary>
        /// Gets existing or new addon according rules.
        /// </summary>
        /// <returns></returns>
        IDataAddon GetAddon();
    }
}
