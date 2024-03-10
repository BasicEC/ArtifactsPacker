using ArtifactsPacker.Services;
using FluentValidation;

namespace ArtifactsPacker.Commands;

public class PackCommand : ICommand
{
    private readonly IPackService _packService;
    private readonly IValidator<PackCommand> _validator;
    
    internal readonly string Src;
    internal readonly string Trg;

    public PackCommand(string src, string trg, IPackService packService, IValidator<PackCommand> validator)
    {
        Src = src;
        Trg = trg;
        _packService = packService;
        _validator = validator;
    }

    public async Task ExecuteAsync()
    {
        await _validator.ValidateAndThrowAsync(this);
        await _packService.CalcHashesAsync(Src);
        await _packService.PackAsync(Src, Trg);
    }
}