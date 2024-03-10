namespace ArtifactsPacker.FileSystem;

public class PhysicalFileSystem : IFileSystemReader, IFileSystemWriter
{
    public IEnumerable<string> EnumerateAllFiles(string path, out int basePathLength)
    {
        var dir = new DirectoryInfo(path);
        basePathLength = dir.FullName.Length + 1;
        return dir.EnumerateFileSystemInfos("*", SearchOption.AllDirectories).Select(f => f.FullName);
    }

    public Stream OpenRead(string basePath, string path) => File.OpenRead(Path.Combine(basePath, path));

    public Stream Create(string basePath, string path) => File.Create(Path.Combine(basePath, path));
}
