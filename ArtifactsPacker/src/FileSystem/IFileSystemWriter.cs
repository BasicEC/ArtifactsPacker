namespace ArtifactsPacker.FileSystem;

public interface IFileSystemWriter
{
    Stream Create(string basePath, string path);
}