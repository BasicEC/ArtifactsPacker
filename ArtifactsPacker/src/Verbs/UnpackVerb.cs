using CommandLine;

namespace ArtifactsPacker.Verbs;

[Verb("unpack", HelpText = "Unpack files in specified directory.")]
public class UnpackVerb : IVerb
{
    [Option("src", Required = true, HelpText = "Path to a directory to unpack")]
    public string Src { get; set; } = null!;

    [Option("trg", Required = true, HelpText = "Path to a directory to place a result")]
    public string Trg { get; set; } = null!;
    
    [Option("archive", FlagCounter = true, HelpText = "Read files to unpack from an archive")]
    public int Archive { get; set; }
}