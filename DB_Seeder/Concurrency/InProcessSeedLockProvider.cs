namespace DB_Seeder.Concurrency;

/// <summary>
/// Thread-safe in-process synchronization lock preventing concurrent seeding threads
/// from colliding inside the same application instance.
/// </summary>
public sealed class InProcessSeedLockProvider : ISeedLockProvider
{
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    public async Task<IAsyncDisposable> AcquireLockAsync(CancellationToken cancellationToken = default)
    {
        await Semaphore.WaitAsync(cancellationToken);
        return new Releaser(Semaphore);
    }

    private sealed class Releaser : IAsyncDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private int _disposed;

        public Releaser(SemaphoreSlim semaphore) => _semaphore = semaphore;

        public ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                _semaphore.Release();
            }
            return ValueTask.CompletedTask;
        }
    }
}



