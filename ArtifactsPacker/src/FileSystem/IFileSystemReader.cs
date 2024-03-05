namespace ArtifactsPacker.FileSystem;

public interface IFileSystemReader
{
    IEnumerable<string> EnumerateFilesAllFiles(string path);
    Stream OpenRead(string basePath, string path);
}