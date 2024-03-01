namespace ArtifactsPacker.Commands;

public class UnpackCommand : ICommand
{
    private readonly string _path;

    public UnpackCommand(string path)
    {
        _path = path;
    }

    public void Execute()
    {
        Console.WriteLine($"Unpack Done! ({_path})");
    }
}