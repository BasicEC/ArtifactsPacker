using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ArtifactsPacker.Services;

public class PackService
{
    internal const string FilesMapName = "filesMap.json";
    
    private DirectoryInfo? _sourceDir;
    private DirectoryInfo? _targetDir;
    internal Dictionary<string, List<FileInfo>>? Hashes;
    
    public void SetSourcePath(string path)
    {
        var dir = new DirectoryInfo(path);
        if (!dir.Exists)
        {
            throw new Exception("Directory doesn't exist");// add validation
        }

        _sourceDir = dir;
    }

    public void SetTargetPath(string path)
    {
        _targetDir = new DirectoryInfo(path);
    
        if (!_targetDir.Exists)
        {
            throw new Exception($"Directory doesn't exist: {_targetDir.FullName}");
        }
    }

    public void CalcHashes()
    {
        if (_sourceDir == null)
        {
            throw new InvalidOperationException("Source directory is not set");
        }

        Hashes = new Dictionary<string, List<FileInfo>>();
        
        foreach (var file in _sourceDir.EnumerateFiles("*", SearchOption.AllDirectories))
        {
            using var stream = file.OpenRead();
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(stream);
            var hex = ToHex(hash);
            if (Hashes.TryGetValue(hex, out var value))
            {
                value.Add(file);
            }
            else
            {
                Hashes[hex] = new List<FileInfo> { file };
            }
        }
    }
    
    public void Pack()
    {
        if (_sourceDir == null)
        {
            throw new InvalidOperationException("Source directory is not set");
        }

        if (_targetDir == null)
        {
            throw new InvalidOperationException("Target directory is not set");
        }

        if (Hashes == null)
        {
            throw new InvalidOperationException("File hashes are not calculated");
        }

        var basePathLen = _sourceDir.FullName.Length + 1;
        var filesMap = new Dictionary<string, string[]>(Hashes.Count);
        foreach (var hash in Hashes.Keys)
        {
            var files = Hashes[hash];
            using (var target = File.Create(Path.Combine(_targetDir.FullName, hash)))
            using (var source = files[0].OpenRead())
            {
                source.CopyTo(target);
            }

            var paths = new string[files.Count];
            for (var i = 0; i < files.Count; i++)
            {
                paths[i] = files[i].FullName[basePathLen..];
            }

            filesMap[hash] = paths;
        }

        using var stream = File.Create(Path.Combine(_targetDir.FullName, FilesMapName));
        JsonSerializer.Serialize(stream, filesMap, new JsonSerializerOptions
        {
            WriteIndented = true,
        });
    }

    public void Unpack()
    {
        if (_sourceDir == null)
        {
            throw new InvalidOperationException("Source directory is not set");
        }

        if (_targetDir == null)
        {
            throw new InvalidOperationException("Target directory is not set");
        }

        var mapFilePath = Path.Combine(_sourceDir.FullName, FilesMapName);
        if (!File.Exists(mapFilePath))
        {
            throw new Exception($"{FilesMapName} is not found {mapFilePath}");
        }

        Dictionary<string, string[]>? map;
        using (var mapFile = File.OpenRead(mapFilePath))
        {
            map = JsonSerializer.Deserialize<Dictionary<string, string[]>>(mapFile);
        }

        if (map == null)
        {
            throw new Exception($"{FilesMapName} is corrupted");
        }

        foreach (var (hash, files) in map)
        {
            foreach (var file in files)
            {
                using (var src = File.OpenRead(Path.Combine(_sourceDir.FullName, hash)))
                using (var trg = File.Create(Path.Combine(_targetDir.FullName, file)))    
                {
                    src.CopyTo(trg);
                }
            }
        }
    }

    private static string ToHex(byte[] bytes)
    {
        var builder = new StringBuilder();
        for (var i = 0; i < bytes.Length; i++)
        {
            builder.Append($"{bytes[i]:x2}");
        }

        return builder.ToString();
    }
}