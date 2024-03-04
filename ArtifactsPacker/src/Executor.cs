using ArtifactsPacker.Commands;
using Microsoft.Extensions.Logging;

namespace ArtifactsPacker;

public interface IExecutor
{
    Task ExecuteAsync(ICommand command);
}

public class Executor : IExecutor
{
    private readonly ILogger<Executor> _logger;

    public Executor(ILogger<Executor> logger)
    {
        _logger = logger;
    }

    public Task ExecuteAsync(ICommand command)
    {
        return command.ExecuteAsync();
    }
}