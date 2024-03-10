using System.IO.Compression;

namespace ArtifactsPacker.FileSystem;

/// <summary>
/// Implementation of <see cref="IFileSystemReader"/> that allows you to enumerate and read files from a zip archive.
/// </summary>
/// <remarks>
/// Multi-threaded reading is not supported.
/// </remarks>
public sealed class ZipFileSystemReader : IFileSystemReader, IDisposable
{
    private readonly Dictionary<string, ZipArchive> _zips = new();

    public IEnumerable<string> EnumerateAllFiles(string path, out int basePathLength)
    {
        if (!_zips.TryGetValue(path, out var zip))
        {
            zip = ZipFile.Open(path, ZipArchiveMode.Read);
            _zips[path] = zip;
        }

        basePathLength = path.Length;
        return zip.Entries.Select(entry => Path.Combine(path, entry.FullName));
    }

    public Stream OpenRead(string basePath, string path)
    {
        if (!_zips.TryGetValue(basePath, out var zip))
        {
            zip = ZipFile.Open(basePath, ZipArchiveMode.Read);
            _zips[path] = zip;
        }

        var entry = zip.GetEntry(path);
        if (entry == null)
        {
            throw new FileNotFoundException("Archive entry is not found", path);
        }

        return entry.Open();
    }

    public void Dispose()
    {
        foreach (var (_, zip) in _zips)
        {
            zip.Dispose();
        }
    }
}
