using ArtifactsPacker.Verbs;
using Microsoft.Extensions.Logging;

namespace ArtifactsPacker;

public interface IVerbProcessor
{
    Task ProcessAsync(IVerb verb);
}

public class VerbProcessor : IVerbProcessor
{
    private readonly ICommandCreator _commandCreator;
    private readonly IExecutor _executor;
    private readonly ILogger<VerbProcessor> _logger;

    public VerbProcessor(ICommandCreator commandCreator, IExecutor executor, ILogger<VerbProcessor> logger)
    {
        _commandCreator = commandCreator;
        _executor = executor;
        _logger = logger;
    }

    public Task ProcessAsync(IVerb verb)
    {
        var command = _commandCreator.Create(verb);
        return _executor.ExecuteAsync(command);
    }
}