using CommandLine;

namespace ArtifactsPacker;

public class ProgramArgs
{
    [Option('p', "pack", Default = false, HelpText = "Pack files in specified directory")]
    public bool Pack { get; set; }

    [Option('u', "unpack", Default = false, HelpText = "Unpack files in specified directory")]
    public bool Unpack { get; set; }

    // CommandLineParser fills this prop
    [Option("path", Required = true, HelpText = "Path to a directory to work with")]
    public string Path { get; set; } = null!; 
}