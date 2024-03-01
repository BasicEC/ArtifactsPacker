using CommandLine;

namespace ArtifactsPacker.Verbs;

[Verb("unpack", HelpText = "Unpack files in specified directory.")]
public class UnpackVerb : IVerb
{
    // CommandLineParser fills this prop
    [Option("path", Required = true, HelpText = "Path to a directory to work with")]
    public string Path { get; set; } = null!; 
}