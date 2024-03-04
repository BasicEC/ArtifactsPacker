﻿using ArtifactsPacker.Services;

namespace ArtifactsPacker.Commands;

public class PackCommand : ICommand
{
    private readonly string _src;
    private readonly string _trg;
    private readonly IPackService _packService;

    public PackCommand(string src, string trg, IPackService packService)
    {
        _src = src;
        _trg = trg;
        _packService = packService;
    }

    public async Task ExecuteAsync()
    {
        _packService.SetSourcePath(_src);
        _packService.SetTargetPath(_trg);
        await _packService.CalcHashesAsync();
        await _packService.PackAsync();
    }
}