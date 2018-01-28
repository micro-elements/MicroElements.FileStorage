using System.IO;
using System.IO.Compression;
using MicroElements.FileStorage.StorageEngine;
using FluentAssertions;
using Xunit;
using System.Linq;
using System;
using MicroElements.FileStorage.Abstractions;
using System.Threading.Tasks;
using MicroElements.FileStorage.ZipEngine;

namespace MicroElements.FileStorage.Tests
{
    public class ZipStorageEngineTests
    {
        private class ZipArchiveCreater
        {
            private readonly MemoryStream _zipStream;
            private ZipArchive _zipArchive;

            internal ZipArchiveCreater()
            {
                _zipStream = new MemoryStream();
                _zipArchive = new ZipArchive(_zipStream, ZipArchiveMode.Create, true);
            }

            internal void AddFile(string fullName, string content)
            {
                var zipEntry = _zipArchive.CreateEntry(fullName, CompressionLevel.NoCompression);
                using (var zipStream = zipEntry.Open())
                {
                    using (var writeStream = new StreamWriter(zipStream))
                    {
                        writeStream.Write(content);
                    }
                }
            }

            internal Stream ZipStream
            {
                get
                {
                    _zipArchive.Dispose();
                    var stream = new MemoryStream();
                    _zipStream.Seek(0, SeekOrigin.Begin);
                    _zipStream.CopyTo(stream, 4096);
                    _zipStream.Seek(0, SeekOrigin.End);
                    stream.Seek(0, SeekOrigin.Begin);
                    _zipArchive = new ZipArchive(_zipStream, ZipArchiveMode.Create, true);
                    return stream;
                }
            }
        }

        [Fact]
        public void create_storageEngine_ReadMode_Fail()
        {
            var stream = new MemoryStream();
            Action createZipStorageEngine = () => { new ZipStorageEngine(new ZipStorageConfiguration(stream) { Mode = ZipStorageEngineMode.Read }); };
            var data = createZipStorageEngine.Should().Throw<InvalidDataException>();
        }

        [Fact]
        public void create_storageEngine_ReadWriteMode()
        {
            var stream = new MemoryStream();
            Action createZipStorageEngine = () => { new ZipStorageEngine(new ZipStorageConfiguration(stream) { Mode = ZipStorageEngineMode.Write }); };
            createZipStorageEngine.Should().NotThrow();
        }

        [Fact]
        public void read_file()
        {
            var zipArchiveCreater = new ZipArchiveCreater();

            var fileName = "test.json";
            var fileContent = "testData";
            zipArchiveCreater.AddFile(fileName, fileContent);
            var zipStorageEngine = new ZipStorageEngine(new ZipStorageConfiguration(zipArchiveCreater.ZipStream));

            var fileFromZipStorage = zipStorageEngine.ReadFile(fileName).GetAwaiter().GetResult();
            fileFromZipStorage.Content.Should().Be(fileContent);
        }

        [Fact]
        public void read_folder()
        {
            var fileContents = new FileContent[]
            {
                new FileContent("1\test0.json", "testData0"),
                new FileContent("1\test1.json", "testData1"),
                new FileContent("1\test2.json", "testData2"),
            };

            var zipArchiveCreater = new ZipArchiveCreater();
            foreach (var file in fileContents)
            {
                zipArchiveCreater.AddFile(file.Location, file.Content);
            }

            var zipStorageEngine = new ZipStorageEngine(new ZipStorageConfiguration(zipArchiveCreater.ZipStream));
            var filesFromZipStorage = zipStorageEngine.ReadDirectory("1").Select(p => p.GetAwaiter().GetResult()).ToArray();
            foreach (var file in fileContents)
            {
                file.Invoking(f =>
                {
                    var fileForCompare = fileContents.FirstOrDefault(p => p.Location == f.Location);
                    fileForCompare.Should().NotBeNull();
                    f.Location.Should().Be(fileForCompare.Location);
                    f.Content.Should().Be(fileForCompare.Content);
                });
                zipArchiveCreater.AddFile(file.Location, file.Content);
            }
            filesFromZipStorage.Count().Should().Be(3);
        }

        [Fact]
        public void read_folder_with_otherFolder()
        {
            var fileContents = new FileContent[]
            {
                new FileContent("1\test0.json", "testData0"),
                new FileContent("1\test1.json", "testData1"),
                new FileContent("1\test2.json", "testData2"),
                new FileContent("2\test3.json", "testData3"),
                new FileContent("test4.json", "testData4"),
            };

            var zipArchiveCreater = new ZipArchiveCreater();
            foreach (var file in fileContents)
            {
                zipArchiveCreater.AddFile(file.Location, file.Content);
            }

            var zipStorageEngine = new ZipStorageEngine(new ZipStorageConfiguration(zipArchiveCreater.ZipStream));
            var filesFromZipStorage = zipStorageEngine.ReadDirectory("1").Select(p => p.GetAwaiter().GetResult()).ToArray();
            foreach (var file in fileContents)
            {
                file.Invoking(f =>
                {
                    var fileForCompare = fileContents.FirstOrDefault(p => p.Location == f.Location);
                    fileForCompare.Should().NotBeNull();
                    f.Location.Should().Be(fileForCompare.Location);
                    f.Content.Should().Be(fileForCompare.Content);
                });
                //zipArchiveCreater.AddFile(file.Location, file.Content);
            }
            filesFromZipStorage.Where(p => p.Location.Contains("test3") || p.Location.Contains("test4")).Count().Should().Be(0);
            filesFromZipStorage.Count().Should().Be(3);
        }

        [Fact]
        public void write_folder_with_otherFolder()
        {
            var fileContents = new FileContent[]
            {
                new FileContent("1\test0.json", "testData0"),
                new FileContent("1\test1.json", "testData1"),
                new FileContent("1\test2.json", "testData2"),
                new FileContent("2\test3.json", "testData3"),
                new FileContent("test4.json", "testData4"),
            };

            var zipMemoryStream = new MemoryStream();
            var zipStorageEngine = new ZipStorageEngine(new ZipStorageConfiguration(zipMemoryStream) { Mode = ZipStorageEngineMode.Write });

            Task.WaitAll(fileContents.Select(f => zipStorageEngine.WriteFile(f.Location, f)).ToArray());

            var filesFromZipStorage = zipStorageEngine.ReadDirectory("1").Select(p => p.GetAwaiter().GetResult()).ToArray();
            foreach (var file in fileContents)
            {
                file.Invoking(f =>
                {
                    var fileForCompare = fileContents.FirstOrDefault(p => p.Location == f.Location);
                    fileForCompare.Should().NotBeNull();
                    f.Location.Should().Be(fileForCompare.Location);
                    f.Content.Should().Be(fileForCompare.Content);
                });
            }
            filesFromZipStorage.Where(p => p.Location.Contains("test3") || p.Location.Contains("test4")).Count().Should().Be(0);
            filesFromZipStorage.Count().Should().Be(3);
        }

        [Fact]
        public void delete_files()
        {
            var fileContentEmpty = new FileContent(string.Empty, string.Empty);
            var file1 = "1\test1.json";
            var file2 = "test4.json";
            var fileContents = new FileContent[]
            {
                new FileContent("1\test0.json", "testData0"),
                new FileContent(file1, "testData1"),
                new FileContent("1\test2.json", "testData2"),
                new FileContent("2\test3.json", "testData3"),
                new FileContent(file2, "testData4"),
            };

            var zipMemoryStream = new MemoryStream();
            var zipStorageEngine = new ZipStorageEngine(new ZipStorageConfiguration(zipMemoryStream) { Mode = ZipStorageEngineMode.Write });

            Task.WaitAll(fileContents.Select(f => zipStorageEngine.WriteFile(f.Location, f)).ToArray());

            var filesFromZipStorage = zipStorageEngine.ReadDirectory("1").Select(p => p.GetAwaiter().GetResult()).ToArray();
            foreach (var file in fileContents)
            {
                file.Invoking(f =>
                {
                    var fileForCompare = fileContents.FirstOrDefault(p => p.Location == f.Location);
                    fileForCompare.Should().NotBeNull();
                    f.Location.Should().Be(fileForCompare.Location);
                    f.Content.Should().Be(fileForCompare.Content);
                });
            }
            filesFromZipStorage.Count(p => p.Location.Contains("test3") || p.Location.Contains("test4")).Should().Be(0);
            filesFromZipStorage.Count().Should().Be(3);
            zipStorageEngine.ReadFile(file1).Should().NotBeNull();
            zipStorageEngine.ReadFile(file2).Should().NotBeNull();

            // delete file1
            zipStorageEngine.DeleteFile(file1);

            filesFromZipStorage = zipStorageEngine.ReadDirectory("1").Select(p => p.GetAwaiter().GetResult()).ToArray();
            foreach (var file in fileContents.Where(p => p.Location != file1))
            {
                file.Invoking(f =>
                {
                    var fileForCompare = fileContents.FirstOrDefault(p => p.Location == f.Location);
                    fileForCompare.Should().NotBeNull();
                    f.Location.Should().Be(fileForCompare.Location);
                    f.Content.Should().Be(fileForCompare.Content);
                });
            }
            filesFromZipStorage.SingleOrDefault(p => p.Location == file1).Should().BeNull();

            filesFromZipStorage.Count(p => p.Location.Contains("test3") || p.Location.Contains("test4")).Should().Be(0);
            filesFromZipStorage.Count().Should().Be(2);
            var ff = zipStorageEngine.ReadFile(file1).GetAwaiter().GetResult();
            zipStorageEngine.ReadFile(file1).GetAwaiter().GetResult().Should().Be(fileContentEmpty);
            zipStorageEngine.ReadFile(file2).GetAwaiter().GetResult().Should().NotBe(fileContentEmpty);

            // delete file1 and file2
            zipStorageEngine.DeleteFile(file2);

            filesFromZipStorage = zipStorageEngine.ReadDirectory("1").Select(p => p.GetAwaiter().GetResult()).ToArray();
            foreach (var file in fileContents.Where(p => p.Location != file1))
            {
                file.Invoking(f =>
                {
                    var fileForCompare = fileContents.FirstOrDefault(p => p.Location == f.Location);
                    fileForCompare.Should().NotBeNull();
                    f.Location.Should().Be(fileForCompare.Location);
                    f.Content.Should().Be(fileForCompare.Content);
                });
            }
            filesFromZipStorage.SingleOrDefault(p => p.Location == file1).Should().BeNull();

            filesFromZipStorage.Count(p => p.Location.Contains("test3") || p.Location.Contains("test4")).Should().Be(0);
            filesFromZipStorage.Count().Should().Be(2);
            zipStorageEngine.ReadFile(file1).GetAwaiter().GetResult().Should().Be(fileContentEmpty);
            zipStorageEngine.ReadFile(file2).GetAwaiter().GetResult().Should().Be(fileContentEmpty);

        }
    }
}
