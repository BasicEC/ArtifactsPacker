using ArtifactsPacker.Verbs;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArtifactsPacker;

public static class Program
{
    public static Task Main(string[] args)
    {
        return Parser.Default.ParseArguments<PackVerb, UnpackVerb>(args)
            .WithParsedAsync(async option =>
            {
                var verb = (IVerb)option;
                var serviceProvider = ConfigureServices(verb);
                var processor = serviceProvider.GetRequiredService<IVerbProcessor>();
                await processor.ProcessAsync();
            });
    }

    private static IServiceProvider ConfigureServices(IVerb verb)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, false).Build();
        var services = new ServiceCollection();

        services.AddLogging(builder =>
            {
                builder.AddConfiguration(config.GetSection("Logging"));
                builder.AddSimpleConsole();
            })
            .AddSingleton<IVerbProcessor, VerbProcessor>()
            .AddScoped<IExecutor, Executor>()
            .AddCommand(verb)
            ;

        return services.BuildServiceProvider();
    }
}