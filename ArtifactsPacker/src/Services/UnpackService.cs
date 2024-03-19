using System.Text.Json;
using ArtifactsPacker.FileSystem;
using Microsoft.Extensions.Logging;

namespace ArtifactsPacker.Services;

public class UnpackService : IUnpackService
{
    private readonly IFileSystemWriter _fileSystemWriter;
    private readonly IFileSystemReader _fileSystemReader;
    private readonly ILogger<UnpackService> _logger;

    public UnpackService(ILogger<UnpackService> logger, IFileSystemReader fileSystemReader,
        IFileSystemWriter fileSystemWriter)
    {
        _logger = logger;
        _fileSystemReader = fileSystemReader;
        _fileSystemWriter = fileSystemWriter;
    }

    public async Task UnpackAsync(string sourcePath, string targetPath)
    {
        _logger.LogInformation("Start unpacking");

        Dictionary<string, string[]>? map;
        await using (var mapFile = _fileSystemReader.OpenRead(sourcePath, PackService.FilesMapName))
        {
            map = JsonSerializer.Deserialize<Dictionary<string, string[]>>(mapFile);
        }

        if (map == null)
        {
            throw new Exception($"{PackService.FilesMapName} is corrupted");
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
}
