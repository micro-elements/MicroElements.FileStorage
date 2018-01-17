// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MicroElements.FileStorage.CodeContracts;

namespace MicroElements.FileStorage.Utils
{
    /// <summary>
    /// Расширения для работы с файлами и путями.
    /// </summary>
    public static class FileExtensions
    {
        /// <summary>
        /// Очистка файлового имени. Заменяет невалидные символы на заданный.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <param name="replaceSymbol">Символ замены для невалидных символов.</param>
        /// <returns>Валидное имя файла.</returns>
        public static string CleanFileName([NotNull] this string fileName, char replaceSymbol = '_')
        {
            Check.NotNull(fileName, nameof(fileName));
            if (Path.GetInvalidFileNameChars().Contains(replaceSymbol))
                throw new ArgumentException($"replaceSymbol '{replaceSymbol}' is invalid file name char", nameof(replaceSymbol));

            return string.Join(replaceSymbol.ToString(), fileName.Split(Path.GetInvalidFileNameChars()));
        }

        /// <summary>
        /// Нормализация слешей в файловом пути.
        /// </summary>
        /// <param name="path">Файловый путь.</param>
        /// <returns>Нормализованный путь.</returns>
        public static string PathNormalize([NotNull] this string path)
        {
            Check.NotNull(path, nameof(path));
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Добавление слеша в конец имени, если его там нет.
        /// </summary>
        /// <param name="path">Файловый путь.</param>
        /// <returns>Путь со слешем в конце.</returns>
        public static string AppendSlashInPath([NotNull] this string path)
        {
            Check.NotNull(path, nameof(path));
            return path.EndsWith(@"\") || path.EndsWith(@"/") ? path : path + Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// Получение относительного пути по имени файла и директории относительно которой вычисляется относительный путь.
        /// </summary>
        /// <param name="fileName">Имя файла. Может быть абсолютныи или относительным.</param>
        /// <param name="basePath">Базовый путь, относительно которого вычисляется относительный путь.</param>
        /// <returns>Относительный путь.</returns>
        public static string RelativeTo([NotNull] this string fileName, [NotNull] string basePath)
        {
            Check.NotNull(fileName, nameof(fileName));
            Check.NotNull(basePath, nameof(basePath));

            return Path.GetFullPath(fileName.PathNormalize()).Replace(Path.GetFullPath(basePath.PathNormalize().AppendSlashInPath()), string.Empty);
        }
    }
}
