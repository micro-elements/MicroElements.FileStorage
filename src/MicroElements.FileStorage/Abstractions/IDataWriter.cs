// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Data writer.
    /// </summary>
    public interface IDataWriter
    {
        /// <summary>
        /// Writes file to destination.
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task WriteFile([NotNull] string basePath, [NotNull] FileContent content);
    }
}