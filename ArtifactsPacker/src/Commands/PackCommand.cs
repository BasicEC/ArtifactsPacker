using ArtifactsPacker.Services;

namespace ArtifactsPacker.Commands;

public class PackCommand : ICommand
{
    private readonly string _src;
    private readonly string _trg;
    private readonly PackService _packService;

    public PackCommand(string src, string trg, PackService packService)
    {
        _src = src;
        _trg = trg;
        _packService = packService;
    }

    public void Execute()
    {
        _packService.SetSourcePath(_src);
        _packService.SetTargetPath(_trg);
        _packService.CalcHashes();
        _packService.Pack();
    }
}