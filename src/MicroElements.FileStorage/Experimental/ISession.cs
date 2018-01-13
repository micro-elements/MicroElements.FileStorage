// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.Experimental
{
    internal interface ISession
    {
        void Save();
    }

    /// <summary>
    /// Data snapshot represents data state in time.
    /// </summary>
    internal interface IDataSnapshot
    {
    }

    internal class SnapshotInfo
    {
        public DateTime Timestamp { get; set; }
    }

    internal class DataAddon
    {
        public DataCommand[] Rows { get; set; }
    }

    internal class DataCommand
    {
        public Command Command { get; set; }
        public FileContent Content { get; set; }
    }

    internal enum Command
    {
        Read,
        Create,
        Update,
        Delete
    }

    // https://github.com/micro-elements/MicroElements.FileStorage/issues/6
    // GH #6 Issue

    /*
proposal:

* Snapshot is original data
* Addon is snapshot changes: add, update, delete
* It can be many addons
* Addons must be ordered
* Snapshot+addon_1+...+addon_N = new snapshot (compaction)
* 
* Audit: initiator, timestamp, signature
* Bulk?
* Import/Export?
* Backup
* Append only storage?

    */
}
