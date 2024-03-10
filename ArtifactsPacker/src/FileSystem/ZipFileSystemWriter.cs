using System.IO.Compression;

namespace ArtifactsPacker.FileSystem;

/// <summary>
/// Implementation of <see cref="IFileSystemWriter"/> that allows you to create a zip archive and write files to it.
/// </summary>
/// <remarks>
/// Multi-threaded writing is not supported.
/// </remarks>
public sealed class ZipFileSystemWriter : IFileSystemWriter, IDisposable
{
    private readonly Dictionary<string, ZipArchive> _zips = new();

    public Stream Create(string basePath, string path)
    {
        if (!_zips.TryGetValue(basePath, out var zip))
        {
            zip = ZipFile.Open(basePath, ZipArchiveMode.Create);
            _zips[basePath] = zip;
        }

        var entry = zip.CreateEntry(path);

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
