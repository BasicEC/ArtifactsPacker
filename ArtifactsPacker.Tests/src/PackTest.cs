using ArtifactsPacker.FileSystem;
using ArtifactsPacker.Services;

namespace ArtifactsPacker.Tests;

public class PackTest
{
    private DirectoryInfo OutDir { get; set; } = null!;
    private IFileSystemWriter _fileSystemWriter = null!;
    private IFileSystemReader _fileSystemReader = null!;

    [SetUp]
    public void SetUp()
    {
        OutDir = new DirectoryInfo("TestFiles/PackTestOut");
        foreach (var file in OutDir.EnumerateFiles("*", SearchOption.AllDirectories))
        {
            file.Delete();
        }
        
        var fs = new PhysicalFileSystem();
        _fileSystemWriter = fs;
        _fileSystemReader = fs;
    }
    
    [Test]
    public void Pack()
    {
        // arrange
        var service = new PackService(_fileSystemWriter, _fileSystemReader);
        const string src = "TestFiles/PackTestIn";
        service.CalcHashesAsync(src).GetAwaiter().GetResult();
        
        // act
        service.PackAsync(src, OutDir.FullName).GetAwaiter().GetResult();
        
        // assert
        var files = OutDir.EnumerateFiles("*", SearchOption.AllDirectories).ToList();
        service.Hashes.Should().NotBeNull();
        files.Should().HaveCount(service.Hashes!.Count + 1);
        files.Should().Contain(f => service.Hashes.ContainsKey(f.Name) || f.Name == PackService.FilesMapName);
    }
}