using ArtifactsPacker.Services;
using ArtifactsPacker.Verbs;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArtifactsPacker;

public class Program
{
    public static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, false).Build();
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(config.GetSection("Logging"));
            builder.AddSimpleConsole();
        });
        
        services.AddSingleton<ICommandCreator, CommandCreator>();
        services.AddSingleton<IExecutor, Executor>();
        services.AddSingleton<IVerbProcessor, VerbProcessor>();
        services.AddSingleton<IPackService, PackService>();

        var serviceProvider = services.BuildServiceProvider();

        await Parser.Default.ParseArguments<PackVerb, UnpackVerb>(args)
            .WithParsedAsync(async verb =>
            {
                var processor = serviceProvider.GetRequiredService<IVerbProcessor>();
                await processor.ProcessAsync((IVerb)verb);
            });
    }
}