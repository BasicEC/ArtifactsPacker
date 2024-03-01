using CommandLine;

namespace ArtifactsPacker.Verbs;

[Verb("pack", HelpText = "Pack files in specified directory.")]
public class PackVerb : IVerb
{
    // CommandLineParser fills this prop
    [Option("path", Required = true, HelpText = "Path to a directory to work with")]
    public string Path { get; set; } = null!; 
}