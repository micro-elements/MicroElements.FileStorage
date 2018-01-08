// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;

namespace MicroElements.FileStorage
{
    /// <summary>
    /// Data snapshot represents data state in time.
    /// </summary>
    public interface IDataSnapshot
    {
    }

    public class SnapshotInfo
    {
        public DateTime Timestamp { get; set; }
    }

    public interface IDataAddon
    {
    }

    public interface ISession
    {
        void Save();
    }

    public interface IDataValidator { }
    public interface IDataConvertor { }


}
