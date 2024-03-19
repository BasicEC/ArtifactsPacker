namespace ArtifactsPacker.Services;

public interface IUnpackService
{
    Task UnpackAsync(string sourcePath, string targetPath);
}
