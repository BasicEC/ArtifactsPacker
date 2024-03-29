﻿using System.Security.Cryptography;
using ArtifactsPacker.Commands;
using ArtifactsPacker.Commands.Validation;
using ArtifactsPacker.FileSystem;
using ArtifactsPacker.Services;
using ArtifactsPacker.Verbs;
using Microsoft.Extensions.DependencyInjection;

namespace ArtifactsPacker;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommand(this IServiceCollection services, IVerb verb) =>
        verb switch
        {
            PackVerb packVerb => services.AddPackCommand(packVerb),
            UnpackVerb unpackVerb => services.AddUnpackCommand(unpackVerb),
            _ => throw new ArgumentOutOfRangeException(nameof(verb)),
        };

    private static IServiceCollection AddPackCommand(this IServiceCollection services, PackVerb verb)
    {
        services.AddScoped<IPackService, PackService>();
        services.AddScoped<HashAlgorithm>(_ => SHA256.Create());

        var archiveResult = verb.Archive > 0;
        services.AddScoped<ICommand>(p =>
            new PackCommand(verb.Src,
                verb.Trg,
                p.GetRequiredService<IPackService>(),
                new PackCommandValidator(archiveResult)));

        services.AddScoped<IFileSystemReader, PhysicalFileSystem>();

        if (archiveResult)
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
        services.AddScoped<IUnpackService, UnpackService>();

        var readFromArchive = verb.Archive > 0;
        services.AddScoped<ICommand>(p =>
            new UnpackCommand(verb.Src,
                verb.Trg,
                p.GetRequiredService<IUnpackService>(),
                new UnpackCommandValidator(readFromArchive)));

        services.AddScoped<IFileSystemWriter, PhysicalFileSystem>();

        if (readFromArchive)
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
