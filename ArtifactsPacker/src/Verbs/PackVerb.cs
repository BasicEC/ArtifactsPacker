using CommandLine;

namespace ArtifactsPacker.Verbs;

[Verb("pack", HelpText = "Pack files in specified directory.")]
public class PackVerb : IVerb
{
    [Option("src", Required = true, HelpText = "Path to a directory to pack")]
    public string Src { get; set; } = null!;

    [Option("trg", Required = true, HelpText = "Path to a directory to place a result")]
    public string Trg { get; set; } = null!;
}