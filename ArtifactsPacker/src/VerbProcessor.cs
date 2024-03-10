using ArtifactsPacker.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArtifactsPacker;

public interface IVerbProcessor
{
    Task ProcessAsync();
}

public class VerbProcessor : IVerbProcessor
{
    private readonly ILogger<VerbProcessor> _logger;
    private readonly IServiceProvider _serviceProvider;

    public VerbProcessor(IServiceProvider serviceProvider, ILogger<VerbProcessor> logger)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task ProcessAsync()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var executor = scope.ServiceProvider.GetRequiredService<IExecutor>();
        var command = scope.ServiceProvider.GetRequiredService<ICommand>();
        await executor.ExecuteAsync(command);
    }
}