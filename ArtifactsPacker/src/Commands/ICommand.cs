namespace ArtifactsPacker.Commands;

public interface ICommand
{
    Task ExecuteAsync();
}