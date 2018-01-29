// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Threading.Tasks;
using MicroElements.FileStorage.CodeContracts;

namespace MicroElements.FileStorage.Utils
{
    /// <summary>
    /// Async file utils.
    /// </summary>
    public class FileAsync
    {
        public static async Task<string> ReadAllText(string path)
        {
            Check.NotNull(path, nameof(path));

            using (var reader = File.OpenText(path))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public static async Task WriteAllText(string path, string content)
        {
            Check.NotNull(path, nameof(path));

            using (var streamWriter = new StreamWriter(path, append: false))
            {
                await streamWriter.WriteAsync(content);
            }
        }
    }
}
