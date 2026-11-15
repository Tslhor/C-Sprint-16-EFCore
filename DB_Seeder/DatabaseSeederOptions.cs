namespace DB_Seeder;

public sealed class DatabaseSeederOptions
{
    public int CustomersToSeed { get; init; } = 10;
    public int SupermarketsToSeed { get; init; } = 5;
    public int ProductsToSeed { get; init; } = 15;
    public int OrdersToSeed { get; init; } = 5;
    public int ItemsPerOrder { get; init; } = 6;
    public decimal DefaultItemQuantity { get; init; } = 3.000m;
    public int MaxRetries { get; init; } = 3;
    public TimeSpan RetryDelay { get; init; } = TimeSpan.FromMilliseconds(250);
}



