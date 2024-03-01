using ArtifactsPacker.Verbs;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArtifactsPacker;

public class Program
{
    public static void Main(string[] args)
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

        var serviceProvider = services.BuildServiceProvider();

        Parser.Default.ParseArguments<PackVerb, UnpackVerb>(args)
            .WithParsed(verb =>
            {
                var processor = serviceProvider.GetRequiredService<IVerbProcessor>();
                processor.Process((IVerb)verb);
            });
    }
}