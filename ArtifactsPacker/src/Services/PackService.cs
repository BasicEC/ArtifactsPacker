using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ArtifactsPacker.FileSystem;
using Microsoft.Extensions.Logging;

namespace ArtifactsPacker.Services;

public class PackService : IPackService
{
    private const int MaxTasksCount = 8;
    internal const string FilesMapName = "filesMap.json";

    private readonly IFileSystemWriter _fileSystemWriter;
    private readonly IFileSystemReader _fileSystemReader;
    private readonly ILogger<PackService> _logger;

    internal IReadOnlyDictionary<string, List<string>>? Hashes;

    public PackService(IFileSystemWriter fileSystemWriter, IFileSystemReader fileSystemReader,
        ILogger<PackService> logger)
    {
        _fileSystemWriter = fileSystemWriter;
        _fileSystemReader = fileSystemReader;
        _logger = logger;
    }

    public async Task CalcHashesAsync(string sourcePath)
    {
        _logger.LogInformation("Start hash calculation");
        var queue = new ConcurrentQueue<(string, string)>();
        var tasks = new ConcurrentQueue<Task>();
        using var semaphore = new SemaphoreSlim(MaxTasksCount);
        var handler = new SemaphoreKeeper(semaphore);
        foreach (var file in _fileSystemReader.EnumerateAllFiles(sourcePath, out var basePathLen))
        {
            using var holder = await handler.WaitAsync();
            var task = Task.Run(async () =>
            {
                using var _ = await handler.WaitAsync();
                var relativePath = file[basePathLen..];
                await using var stream = _fileSystemReader.OpenRead(sourcePath, relativePath);
                var md5 = MD5.Create();
                var hash = await md5.ComputeHashAsync(stream);
                var hex = ToHex(hash);
                queue.Enqueue((hex, relativePath));
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

        _logger.LogInformation("Hash calculation complete");
    }

    public async Task PackAsync(string sourcePath, string targetPath)
    {
        _logger.LogInformation("Start packing");

        if (Hashes == null)
        {
            throw new InvalidOperationException("File hashes are not calculated");
        }

        foreach (var hash in Hashes.Keys)
        {
            var files = Hashes[hash];
            await using var target = _fileSystemWriter.Create(targetPath, hash);
            await using var source = _fileSystemReader.OpenRead(sourcePath, files[0]);
            await source.CopyToAsync(target);
        }

        await using var stream = _fileSystemWriter.Create(targetPath, FilesMapName);
        await JsonSerializer.SerializeAsync(stream, Hashes, new JsonSerializerOptions { WriteIndented = true, });

        _logger.LogInformation("Packing complete");
    }

    public async Task UnpackAsync(string sourcePath, string targetPath)
    {
        _logger.LogInformation("Start unpacking");

        Dictionary<string, string[]>? map;
        await using (var mapFile = _fileSystemReader.OpenRead(sourcePath, FilesMapName))
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
                await using var src = _fileSystemReader.OpenRead(sourcePath, hash);
                await using var trg = _fileSystemWriter.Create(targetPath, file);
                await src.CopyToAsync(trg);
            }
        }

        _logger.LogInformation("Unpacking complete");
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
