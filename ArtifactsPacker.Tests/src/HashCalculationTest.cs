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

    [Test]
    public void HashConvertsToStringCorrectly()
    {
        var bytes = new byte[]
        {
            0x0f, 0x1e, 0x2d, 0x3c, 0x4b, 0x5a, 0x69, 0x78,
            0x87, 0x96, 0xa5, 0xb4, 0xc3, 0xd2, 0xe1, 0xf0,
        };
        var result = HexConverter.Convert(bytes);
        result.Should().Be("0f1e2d3c4b5a69788796a5b4c3d2e1f0");
    }
}
