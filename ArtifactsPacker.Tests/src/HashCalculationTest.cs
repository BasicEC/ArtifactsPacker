using ArtifactsPacker.Services;

namespace ArtifactsPacker.Tests;

public class HashCalculationTest
{
    [Test]
    public void HashesAreEqualForTheSameFiles()
    {
        var service = new PackService();
        service.SetSourcePath("TestFiles/PackTestIn");
        service.CalcHashesAsync().GetAwaiter().GetResult();

        service.Hashes.Should().NotBeEmpty();
        service.Hashes!.Count.Should().Be(2);
        service.Hashes["e00ca641d60d9473686d6c5b5fb67a7b"].Should().HaveCount(2);
        service.Hashes["427a39f4a46108dcbf441fb0f827baca"].Should().HaveCount(1);
    }

    [Test]
    public void ThrowsIfSourceDirectoryIsNotSet()
    {
        var action = () => new PackService().CalcHashesAsync().GetAwaiter().GetResult();

        action.Should().Throw<InvalidOperationException>()
            .And.Message.Should().Be("Source directory is not set");
    }
}