using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ArtifactsPacker.Services;

public class PackService : IPackService
{
    private const int MaxTasksCount = 8;
    internal const string FilesMapName = "filesMap.json";
    
    private DirectoryInfo? _sourceDir;
    private DirectoryInfo? _targetDir;
    internal IReadOnlyDictionary<string, List<string>>? Hashes;
    
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

    public async Task CalcHashesAsync()
    {
        if (_sourceDir == null)
        {
            throw new InvalidOperationException("Source directory is not set");
        }

        var queue = new ConcurrentQueue<(string, string)>();
        var tasks = new ConcurrentQueue<Task>();
        using var semaphore = new SemaphoreSlim(MaxTasksCount);
        var handler = new SemaphoreKeeper(semaphore);
        foreach (var file in _sourceDir.EnumerateFiles("*", SearchOption.AllDirectories))
        {
            using var holder = await handler.WaitAsync();
            var task = Task.Run(async () =>
            {
                using var _ = await handler.WaitAsync();
                await using var stream = file.OpenRead();
                var md5 = MD5.Create();
                var hash = await md5.ComputeHashAsync(stream);
                var hex = ToHex(hash);
                queue.Enqueue((hex, file.FullName));
                if (tasks.Count > MaxTasksCount)
                {
                    tasks.TryDequeue(out var _);
                }
            });
            
            tasks.Enqueue(task);
        }

        await Task.WhenAll(tasks);

        var hashes = new Dictionary<string, List<string>>();
        foreach (var (hex, file) in queue)
        {
            if (hashes.TryGetValue(hex, out var files))
            {
                files.Add(file);
            }
            else
            {
                hashes[hex] = new List<string> { file };
            }
        }

        Hashes = hashes;
    }
    
    public async Task PackAsync()
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
            await using (var target = File.Create(Path.Combine(_targetDir.FullName, hash)))
            await using (var source = File.OpenRead(files[0]))
            {
                await source.CopyToAsync(target);
            }

            var paths = new string[files.Count];
            for (var i = 0; i < files.Count; i++)
            {
                paths[i] = files[i][basePathLen..];
            }

            filesMap[hash] = paths;
        }

        await using var stream = File.Create(Path.Combine(_targetDir.FullName, FilesMapName));
        await JsonSerializer.SerializeAsync(stream, filesMap, new JsonSerializerOptions
        {
            WriteIndented = true,
        });
    }

    public async Task UnpackAsync()
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
        await using (var mapFile = File.OpenRead(mapFilePath))
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
                await using var src = File.OpenRead(Path.Combine(_sourceDir.FullName, hash));
                await using var trg = File.Create(Path.Combine(_targetDir.FullName, file));
                await src.CopyToAsync(trg);
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