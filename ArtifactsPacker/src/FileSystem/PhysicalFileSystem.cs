namespace ArtifactsPacker.FileSystem;

public class PhysicalFileSystem : IFileSystemReader, IFileSystemWriter
{
    public IEnumerable<string> EnumerateFilesAllFiles(string path)
    {
        var dir = new DirectoryInfo(path);
        return dir.EnumerateFileSystemInfos("*", SearchOption.AllDirectories).Select(f => f.FullName);
    }

    public Stream OpenRead(string basePath, string path)
    {
        return File.OpenRead(Path.Combine(basePath, path));
    }

    public Stream Create(string basePath, string path)
    {
        return File.Create(Path.Combine(basePath, path));
    }
}