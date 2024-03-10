namespace ArtifactsPacker;

public sealed class SemaphoreKeeper : IDisposable
{
    private readonly SemaphoreSlim _semaphore;

    public SemaphoreKeeper(SemaphoreSlim semaphore)
    {
        _semaphore = semaphore;
    }

    public async Task<IDisposable> WaitAsync()
    {
        await _semaphore.WaitAsync();
        return this;
    }

    public void Dispose() => _semaphore.Release();
}
