using System.IO.Compression;
using System.Text;
using ArtifactsPacker.FileSystem;

namespace ArtifactsPacker.Tests.Zip;

public class ZipFileSystemWriterTest
{
    private const string TestContent = "test content";
    private static readonly byte[] TestContentBytes = Encoding.UTF8.GetBytes(TestContent);
    private const string OutArchive = "TestFiles/Zip/Out/test.zip";

    [SetUp]
    public void SetUp()
    {
        var outFile = new FileInfo(OutArchive);
        if (outFile.Exists)
        {
            outFile.Delete();
        }
    }

    [Test]
    public void ArchiveIsCreatedCorrectly()
    {
        // act
        string[] files;
        using (var writer = new ZipFileSystemWriter())
        {
            files = Enumerable.Range(0, 200).Select(i =>
            {
                var fileName = $"{i}.txt";
                using var stream = writer.Create(OutArchive, fileName);
                stream.Write(TestContentBytes);
                return fileName;
            }).ToArray();
        }

        // assert
        var zip = ZipFile.OpenRead(OutArchive);
        zip.Entries.Should().HaveCount(files.Length);
        foreach (var file in files)
        {
            var entry = zip.GetEntry(file);
            AssertEntry(entry);
        }
    }

    private static void AssertEntry(ZipArchiveEntry? entry)
    {
        entry.Should().NotBeNull();
        using var stream = new StreamReader(entry!.Open());
        var content = stream.ReadToEnd();
        content.Should().Be(TestContent);
    }
}
