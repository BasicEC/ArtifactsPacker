namespace ArtifactsPacker.FileSystem;

public class PhysicalFileSystem : IFileSystemReader, IFileSystemWriter
{
    public IEnumerable<string> EnumerateAllFiles(string path, out int basePathLength)
    {
        var dir = new DirectoryInfo(path);
        basePathLength = dir.FullName.Length + 1;
        return dir.EnumerateFileSystemInfos("*", SearchOption.AllDirectories)
            .Where(f => !f.Attributes.HasFlag(FileAttributes.Directory))
            .Select(f => f.FullName);
    }

    public Stream OpenRead(string basePath, string path) => File.OpenRead(Path.Combine(basePath, path));

    public Stream Create(string basePath, string path)
    {
        var fullBasePath = Path.GetFullPath(basePath);
        var fullPath = Path.Combine(fullBasePath, path);
        var file = new FileInfo(fullPath);
        var dir = file.Directory;
        if (dir is { Exists: false })
        {
            dir.Create();
        }

        return file.Create();
    }
}
