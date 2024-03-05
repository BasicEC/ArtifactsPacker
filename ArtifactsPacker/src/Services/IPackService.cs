namespace ArtifactsPacker.Services;

public interface IPackService
{
    Task CalcHashesAsync(string sourcePath);
    Task PackAsync(string sourcePath, string targetPath);
    Task UnpackAsync(string sourcePath, string targetPath);
}