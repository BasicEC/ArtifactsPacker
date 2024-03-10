namespace ArtifactsPacker.FileSystem;

public interface IFileSystemReader
{
    IEnumerable<string> EnumerateAllFiles(string path, out int basePathLength);
    Stream OpenRead(string basePath, string path);
}
