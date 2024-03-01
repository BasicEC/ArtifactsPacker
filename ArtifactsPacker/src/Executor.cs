using ArtifactsPacker.Commands;
using Microsoft.Extensions.Logging;

namespace ArtifactsPacker;

public interface IExecutor
{
    void Execute(ICommand command);
}

public class Executor : IExecutor
{
    private readonly ILogger<Executor> _logger;

    public Executor(ILogger<Executor> logger)
    {
        _logger = logger;
    }

    public void Execute(ICommand command)
    {
        command.Execute();
    }
}