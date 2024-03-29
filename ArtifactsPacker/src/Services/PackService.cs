﻿using System.Security.Cryptography;
using System.Text.Json;
using ArtifactsPacker.FileSystem;
using Microsoft.Extensions.Logging;

namespace ArtifactsPacker.Services;

public class PackService : IPackService
{
    internal const string FilesMapName = "filesMap.json";

    private readonly IFileSystemWriter _fileSystemWriter;
    private readonly IFileSystemReader _fileSystemReader;
    private readonly ILogger<PackService> _logger;
    private readonly HashAlgorithm _hashAlgorithm;

    internal IReadOnlyDictionary<string, List<string>>? Hashes;

    public PackService(IFileSystemWriter fileSystemWriter,
        IFileSystemReader fileSystemReader,
        ILogger<PackService> logger,
        HashAlgorithm hashAlgorithm)
    {
        _fileSystemWriter = fileSystemWriter;
        _fileSystemReader = fileSystemReader;
        _logger = logger;
        _hashAlgorithm = hashAlgorithm;
    }

    public async Task CalcHashesAsync(string sourcePath)
    {
        _logger.LogInformation("Start hash calculation");
        var hashes = new Dictionary<string, List<string>>();
        foreach (var file in _fileSystemReader.EnumerateAllFiles(sourcePath, out var basePathLen))
        {
            var relativePath = file[basePathLen..];
            await using var stream = _fileSystemReader.OpenRead(sourcePath, relativePath);
            var hash = await _hashAlgorithm.ComputeHashAsync(stream);
            var hex = HexConverter.Convert(hash);
            
            if (hashes.TryGetValue(hex, out var files))
            {
                files.Add(relativePath);
            }
            else
            {
                hashes[hex] = new List<string> { relativePath };
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
}
