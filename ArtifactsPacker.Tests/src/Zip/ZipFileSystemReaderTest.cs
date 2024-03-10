using ArtifactsPacker.FileSystem;

namespace ArtifactsPacker.Tests.Zip;

public class ZipFileSystemReaderTest
{
    private const string TestContent = "test content";
    private const string InArchive = "TestFiles/Zip/In/test.zip";
    
    [Test]
    public void ArchiveIsReadCorrectly()
    {
        using var reader = new ZipFileSystemReader();
        var contents = Enumerable.Range(0, 200).Select(i =>
        {
            var fileName = $"{i}.txt";
            // ReSharper disable once AccessToDisposedClosure
            using var stream = new StreamReader(reader.OpenRead(InArchive, fileName));
            return stream.ReadToEnd();
        });

        foreach (var content in contents)
        {
            content.Should().Be(TestContent);
        }
    }
}