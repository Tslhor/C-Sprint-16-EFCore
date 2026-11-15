namespace DB_Seeder.Concurrency;

public interface ISeedLockProvider
{
    Task<IAsyncDisposable> AcquireLockAsync(CancellationToken cancellationToken = default);
}



