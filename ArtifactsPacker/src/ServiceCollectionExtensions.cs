using ArtifactsPacker.Commands;
using ArtifactsPacker.FileSystem;
using ArtifactsPacker.Services;
using ArtifactsPacker.Verbs;
using Microsoft.Extensions.DependencyInjection;

namespace ArtifactsPacker;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommand(this IServiceCollection services, IVerb verb)
    {
        services.AddScoped<IPackService, PackService>();

        return verb switch
        {
            PackVerb packVerb => services.AddPackCommand(packVerb),
            UnpackVerb unpackVerb => services.AddUnpackCommand(unpackVerb),
            _ => throw new ArgumentOutOfRangeException(nameof(verb)),
        };
    }

    private static IServiceCollection AddPackCommand(this IServiceCollection services, PackVerb verb)
    {
        services.AddScoped<ICommand>(p => new PackCommand(verb.Src, verb.Trg, p.GetRequiredService<IPackService>()));
        services.AddScoped<IFileSystemReader, PhysicalFileSystem>();

        if (verb.Archive > 0)
        {
            services.AddScoped<IFileSystemWriter, ZipFileSystemWriter>();
        }
        else
        {
            services.AddScoped<IFileSystemWriter, PhysicalFileSystem>();
        }

        return services;
    }
    
    private static IServiceCollection AddUnpackCommand(this IServiceCollection services, UnpackVerb verb)
    {
        services.AddScoped<ICommand>(p => new UnpackCommand(verb.Src, verb.Trg, p.GetRequiredService<IPackService>()));
        services.AddScoped<IFileSystemWriter, PhysicalFileSystem>();

        if (verb.Archive > 0)
        {
            services.AddScoped<IFileSystemReader, ZipFileSystemReader>();
        }
        else
        {
            services.AddScoped<IFileSystemReader, PhysicalFileSystem>();
        }

        return services;
    }
}