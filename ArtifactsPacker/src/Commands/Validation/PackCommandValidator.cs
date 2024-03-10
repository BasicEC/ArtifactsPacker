using FluentValidation;

namespace ArtifactsPacker.Commands.Validation;

public class PackCommandValidator : AbstractValidator<PackCommand>
{
    public PackCommandValidator(bool archiveResult)
    {
        RuleFor(c => c.Src)
            .Custom((src, context) =>
            {
                if (Directory.Exists(src)) return;

                var fullname = Path.GetFileName(src);
                context.AddFailure(nameof(PackCommand.Src), $"Source directory doesn't exist: {fullname}");
            });

        RuleFor(c => c.Trg)
            .Custom((trg, context) =>
            {
                if (Directory.Exists(trg)) return;
                var fullname = Path.GetFullPath(trg);
                context.AddFailure(nameof(PackCommand.Trg), $"Target directory doesn't exist: {fullname}");
            }).When(c => !archiveResult);
        
        RuleFor(c => c.Trg)
            .Custom((trg, context) =>
            {
                var ext = Path.GetExtension(trg);
                if (".zip" == ext) return;
                context.AddFailure(nameof(PackCommand.Trg), $"Target archive name must ends with .zip: {trg}");
            })
            .Custom((trg, context) =>
            {
                if (!File.Exists(trg)) return;
                var fullname = Path.GetFullPath(trg);
                context.AddFailure(nameof(PackCommand.Trg), $"Target archive already exists: {fullname}");
            })
            .When(c => archiveResult);
    }
}