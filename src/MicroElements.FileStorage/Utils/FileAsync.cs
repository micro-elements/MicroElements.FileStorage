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

            using (var stream = File.OpenWrite(path))
            using (var streamWriter = new StreamWriter(stream))
            {
                await streamWriter.WriteAsync(content);
            }
        }
    }
}