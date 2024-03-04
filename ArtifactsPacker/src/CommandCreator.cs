using ArtifactsPacker.Commands;
using ArtifactsPacker.Services;
using ArtifactsPacker.Verbs;

namespace ArtifactsPacker;

public interface ICommandCreator
{
    ICommand Create(IVerb verb);
}

public class CommandCreator : ICommandCreator
{
    private readonly PackService _packService;
    
    public CommandCreator(PackService packService)
    {
        _packService = packService;
    }

    public ICommand Create(IVerb verb)
    {
        return verb switch
        {
            PackVerb packVerb => new PackCommand(packVerb.Src, packVerb.Trg, _packService),
            UnpackVerb unpackVerb => new UnpackCommand(unpackVerb.Src, unpackVerb.Trg, _packService),
            _ => throw new ArgumentOutOfRangeException(nameof(verb))
        };
    }
}