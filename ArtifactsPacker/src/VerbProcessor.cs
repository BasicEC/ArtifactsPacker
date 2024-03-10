using ArtifactsPacker.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace ArtifactsPacker;

public interface IVerbProcessor
{
    Task ProcessAsync();
}

public class VerbProcessor : IVerbProcessor
{
    private readonly IServiceProvider _serviceProvider;

    public VerbProcessor(IServiceProvider serviceProvider)
    {
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
