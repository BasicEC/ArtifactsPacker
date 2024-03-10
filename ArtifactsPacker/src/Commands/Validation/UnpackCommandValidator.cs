using FluentValidation;

namespace ArtifactsPacker.Commands.Validation;

public class UnpackCommandValidator : AbstractValidator<UnpackCommand>
{
    public UnpackCommandValidator(bool readFromArchive)
    {
        RuleFor(c => c.Trg).Custom((trg, context) =>
        {
            if (Directory.Exists(trg)) return;

            var fullname = Path.GetFullPath(trg);
            context.AddFailure(nameof(UnpackCommand.Trg), $"Target directory doesn't exist: {fullname}");
        });

        RuleFor(c => c.Src)
            .Custom((src, context) =>
            {
                if (Directory.Exists(src)) return;

                var fullname = Path.GetFullPath(src);
                context.AddFailure(nameof(UnpackCommand.Src), $"Source directory doesn't exist: {fullname}");
            }).When(_ => !readFromArchive);

        RuleFor(c => c.Src)
            .Custom((src, context) =>
            {
                var ext = Path.GetExtension(src);
                if (".zip" == ext) return;
                context.AddFailure(nameof(UnpackCommand.Src), $"Source archive name must ends with .zip: {src}");
            })
            .Custom((src, context) =>
            {
                if (File.Exists(src)) return;
                var fullname = Path.GetFullPath(src);
                context.AddFailure(nameof(UnpackCommand.Src), $"Source archive doesn't exist: {fullname}");
            })
            .When(_ => readFromArchive);
    }
}
