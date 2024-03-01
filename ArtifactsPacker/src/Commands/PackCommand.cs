namespace ArtifactsPacker.Commands;

public class PackCommand : ICommand
{
    private readonly string _path;

    public PackCommand(string path)
    {
        _path = path;
    }

    public void Execute()
    {
        Console.WriteLine($"Pack Done! ({_path})");
    }
}