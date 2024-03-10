using ArtifactsPacker.FileSystem;
using ArtifactsPacker.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtifactsPacker.Tests;

public class HashCalculationTest
{
    private IFileSystemWriter _fileSystemWriter = null!;
    private IFileSystemReader _fileSystemReader = null!;

    [SetUp]
    public void Setup()
    {
        var fs = new PhysicalFileSystem();
        _fileSystemWriter = fs;
        _fileSystemReader = fs;
    }
    
    [Test]
    public void HashesAreEqualForTheSameFiles()
    {
        var service = new PackService(_fileSystemWriter, _fileSystemReader, NullLogger<PackService>.Instance);
        service.CalcHashesAsync("TestFiles/PackTestIn").GetAwaiter().GetResult();

        service.Hashes.Should().NotBeEmpty();
        service.Hashes!.Count.Should().Be(2);
        service.Hashes["e00ca641d60d9473686d6c5b5fb67a7b"].Should().HaveCount(2);
        service.Hashes["427a39f4a46108dcbf441fb0f827baca"].Should().HaveCount(1);
    }
}