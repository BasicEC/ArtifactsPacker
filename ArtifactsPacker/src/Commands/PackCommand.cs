using ArtifactsPacker.Services;

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
        await _packService.CalcHashesAsync(_src);
        await _packService.PackAsync(_src, _trg);
    }
}