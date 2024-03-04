using ArtifactsPacker.Services;

namespace ArtifactsPacker.Tests;

public class PackTest
{
    private DirectoryInfo OutDir { get; set; } = null!;

    [SetUp]
    public void SetUp()
    {
        OutDir = new DirectoryInfo("TestFiles/PackTestOut");
        foreach (var file in OutDir.EnumerateFiles("*", SearchOption.AllDirectories))
        {
            file.Delete();
        }
    }
    
    [Test]
    public void Pack()
    {
        // arrange
        var service = new PackService();
        service.SetSourcePath("TestFiles/PackTestIn");
        service.SetTargetPath(OutDir.FullName);
        service.CalcHashesAsync().GetAwaiter().GetResult();
        
        // act
        service.PackAsync().GetAwaiter().GetResult();
        
        // assert
        var files = OutDir.EnumerateFiles("*", SearchOption.AllDirectories).ToList();
        service.Hashes.Should().NotBeNull();
        files.Should().HaveCount(service.Hashes!.Count + 1);
        files.Should().Contain(f => service.Hashes.ContainsKey(f.Name) || f.Name == PackService.FilesMapName);
    }
}