namespace ArtifactsPacker.FileSystem;

public interface IFileSystemReader
{
    IEnumerable<string> EnumerateAllFiles(string path);
    Stream OpenRead(string basePath, string path);
}
