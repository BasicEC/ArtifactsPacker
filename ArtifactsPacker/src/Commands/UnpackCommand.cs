using ArtifactsPacker.Services;
using FluentValidation;

namespace ArtifactsPacker.Commands;

public class UnpackCommand : ICommand
{
    private readonly IUnpackService _service;
    private readonly IValidator<UnpackCommand> _validator;

    internal readonly string Src;
    internal readonly string Trg;

    public UnpackCommand(string src, string trg, IUnpackService service, IValidator<UnpackCommand> validator)
    {
        Src = src;
        Trg = trg;
        _service = service;
        _validator = validator;
    }

    public async Task ExecuteAsync()
    {
        await _validator.ValidateAndThrowAsync(this);
        await _service.UnpackAsync(Src, Trg);
    }
}
