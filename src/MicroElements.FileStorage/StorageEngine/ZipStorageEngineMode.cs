// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace MicroElements.FileStorage.StorageEngine
{
    /// <summary>
    /// Mode of work with archive
    /// </summary>
    public enum ZipStorageEngineMode
    {
        // Only reading archive is permitted.
        Read,

        // Both read and write operations are permitted for archive.
        Write,
    }
}
