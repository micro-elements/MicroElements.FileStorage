// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace MicroElements.FileStorage.Abstractions
{
    public interface IExportable<T> where T : class
    {
        ExportData<T> Export();
    }

    public interface IImportable<T> where T : class
    {
        void Import(ExportData<T> data);
    }

    public class ExportData<T> where T : class
    {
        public IReadOnlyList<EntityWithKey<T>> Added { get; set; }

        public IReadOnlyList<string> Deleted { get; set; }
    }
}
