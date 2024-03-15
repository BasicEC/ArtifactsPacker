using System.Security.Cryptography;
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
        var service = new PackService(_fileSystemWriter, _fileSystemReader, NullLogger<PackService>.Instance, SHA256.Create());
        service.CalcHashesAsync("TestFiles/PackTestIn").GetAwaiter().GetResult();

        service.Hashes.Should().NotBeEmpty();
        service.Hashes!.Count.Should().Be(2);
        service.Hashes["c3e5a163ff8e90377138cfa256135ee404be457e30edf81c0772efc3d36a8fe3"].Should().HaveCount(3);
        service.Hashes["d6c2eff2362d1257c6978ff37621819cb001745f698ba58f88d07cfec8ed0e2b"].Should().HaveCount(1);
        foreach (var filePath in service.Hashes.Values.SelectMany(_ => _))
        {
            filePath.Should().NotContain("TestFiles");
            filePath.Should().NotContain("PackTestIn");
        }
    }
}
