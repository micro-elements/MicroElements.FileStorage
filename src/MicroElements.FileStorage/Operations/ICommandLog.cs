// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace MicroElements.FileStorage.Operations
{
    public interface IReadOnlyCommandLog : IEnumerable<StoreCommand>
    {
        int Count { get; }
        //object this[int index] { get; }
    }

    public interface ICommandLog : IReadOnlyCommandLog
    {
        void Add(StoreCommand command);
        void BulkAdd(StoreCommand[] commands);
    }
}
