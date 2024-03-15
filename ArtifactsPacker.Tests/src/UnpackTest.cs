using System.Security.Cryptography;
using ArtifactsPacker.FileSystem;
using ArtifactsPacker.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtifactsPacker.Tests;

public class UnpackTest
{
    private DirectoryInfo OutDir { get; set; } = null!;
    private IFileSystemWriter _fileSystemWriter = null!;
    private IFileSystemReader _fileSystemReader = null!;


    [SetUp]
    public void SetUp()
    {
        OutDir = new DirectoryInfo("TestFiles/UnpackTestOut");
        foreach (var file in OutDir.EnumerateFiles("*", SearchOption.AllDirectories))
        {
            file.Delete();
        }

        var fs = new PhysicalFileSystem();
        _fileSystemWriter = fs;
        _fileSystemReader = fs;
    }

    [Test]
    public void Unpack()
    {
        // arrange
        var service = new PackService(_fileSystemWriter, _fileSystemReader, NullLogger<PackService>.Instance, SHA256.Create());
        const string src = "TestFiles/UnpackTestIn";
        var expectedFiles = new Dictionary<string, string>
        {
            { "sameHash1.txt", "08E38238-CC73-48E4-92D7-B4A079651ADE" },
            { "sameHash2.txt", "08E38238-CC73-48E4-92D7-B4A079651ADE" },
            { "uniqHash.txt", "800F769F-4F0E-4E24-876D-3EE3D71B1692" },
        };

        // act
        service.UnpackAsync(src, OutDir.FullName).GetAwaiter().GetResult();

        // assert
        var files = OutDir.EnumerateFiles("*", SearchOption.AllDirectories).ToArray();
        files.Should().HaveCount(4);
        foreach (var file in files)
        {
            expectedFiles.TryGetValue(file.Name, out var expectedContent);
            expectedContent.Should().NotBeNull();
            var content = File.ReadAllText(file.FullName);
            expectedContent.Should().Be(content);
        }
    }
}
