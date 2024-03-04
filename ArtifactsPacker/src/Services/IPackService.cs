namespace ArtifactsPacker.Services;

public interface IPackService
{
    /// <summary>
    /// Set the path where input files will be searched in <see cref="CalcHashesAsync"/>,
    /// <see cref="PackAsync"/> and <see cref="UnpackAsync"/> methods.
    /// </summary>
    void SetSourcePath(string path);
    
    /// <summary>
    /// Set the path where result will be placed in
    /// <see cref="PackAsync"/> and <see cref="UnpackAsync"/> methods.
    /// </summary>
    void SetTargetPath(string path);

    /// <summary>
    /// Calculate file hashes in the specified directory.
    /// Must be called before <see cref="PackAsync"/>.
    /// </summary>
    Task CalcHashesAsync();
    
    /// <summary>
    /// Pack files using calculated hashes.
    /// Must be called after <see cref="CalcHashesAsync"/>.
    /// </summary>
    Task PackAsync();

    /// <summary>
    /// Unpack files in the specified directory.
    /// </summary>
    Task UnpackAsync();
}