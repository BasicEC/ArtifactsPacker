using ArtifactsPacker.Services;
using FluentValidation;

namespace ArtifactsPacker.Commands;

public class UnpackCommand : ICommand
{
    private readonly IPackService _packService;
    private readonly IValidator<UnpackCommand> _validator;

    internal readonly string Src;
    internal readonly string Trg;

    public UnpackCommand(string src, string trg, IPackService packService, IValidator<UnpackCommand> validator)
    {
        Src = src;
        Trg = trg;
        _packService = packService;
        _validator = validator;
    }

    public async Task ExecuteAsync()
    {
        await _validator.ValidateAndThrowAsync(this);
        await _packService.UnpackAsync(Src, Trg);
    }
}
