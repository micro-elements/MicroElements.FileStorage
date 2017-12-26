using JetBrains.Annotations;
using MicroElements.FileStorage.CodeContracts;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Abstraction of file content.
    /// </summary>
    public class FileContent
    {
        /// <summary>
        /// File location.
        /// </summary>
        public string Location { get; }

        /// <summary>
        /// File contens as string.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="location">File location.</param>
        /// <param name="content">File contens as string.</param>
        public FileContent([NotNull] string location, [NotNull] string content)
        {
            Check.NotNull(location, nameof(location));
            Check.NotNull(content, nameof(content));

            Location = location;
            Content = content;
        }
    }
}