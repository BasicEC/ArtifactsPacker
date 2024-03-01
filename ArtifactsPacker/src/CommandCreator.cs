using ArtifactsPacker.Commands;
using ArtifactsPacker.Verbs;

namespace ArtifactsPacker;

public interface ICommandCreator
{
    ICommand Create(IVerb verb);
}

public class CommandCreator : ICommandCreator
{
    public ICommand Create(IVerb verb)
    {
        return verb switch
        {
            PackVerb packVerb => new PackCommand(packVerb.Path),
            UnpackVerb unpackVerb => new UnpackCommand(unpackVerb.Path),
            _ => throw new ArgumentOutOfRangeException(nameof(verb))
        };
    }
}