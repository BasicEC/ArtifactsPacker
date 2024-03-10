using ArtifactsPacker.Commands;
using FluentValidation;
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

    public async Task ExecuteAsync(ICommand command)
    {
        try
        {
            await command.ExecuteAsync();
        }
        catch (ValidationException e)
        {
            _logger.LogError(e.Message);
        }
    }
}