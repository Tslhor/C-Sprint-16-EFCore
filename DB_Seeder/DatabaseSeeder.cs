using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Data;
using Domain.Entities;
using DB_Seeder.Concurrency;
using DB_Seeder.Models;
using DB_Seeder.Services;

namespace DB_Seeder;

/// <summary>
/// Database-agnostic EF Core seeder supporting SQLite, SQL Server, and other relational providers.
/// Uses EF Core Migrations for schema setup, transactional isolation + retry logic for multi-instance safety,
/// and streaming Reservoir Sampling to bound memory usage regardless of seed file size.
/// </summary>
public sealed class DatabaseSeeder : IDatabaseSeeder
{
    private readonly ShoppingDbContext _dbContext;
    private readonly ISeedDataReader _dataReader;
    private readonly ISeedLockProvider _lockProvider;
    private readonly DatabaseSeederOptions _options;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ShoppingDbContext dbContext,
        ISeedDataReader dataReader,
        ISeedLockProvider lockProvider,
        DatabaseSeederOptions? options = null,
        ILogger<DatabaseSeeder>? logger = null)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
        _lockProvider = lockProvider ?? throw new ArgumentNullException(nameof(lockProvider));
        _options = options ?? new DatabaseSeederOptions();
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DatabaseSeeder>.Instance;
    }

    public async Task SeedIfEmptyAsync(string seedDataDirectory, CancellationToken cancellationToken = default)
    {
        await ExecuteWithConcurrencyControlAsync(async ct =>
        {
            // Apply any pending EF Core migrations safely (EF Core uses __EFMigrationsHistory lock)
            _logger.LogInformation("Applying pending EF Core database migrations...");
            await _dbContext.Database.MigrateAsync(ct);

            // Use Serializable isolation to eliminate TOCTOU race conditions across multiple processes/instances
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);

            try
            {
                var hasBaseData = await _dbContext.Products.AnyAsync(ct)
                                   && await _dbContext.Customers.AnyAsync(ct)
                                   && await _dbContext.Supermarkets.AnyAsync(ct);

                if (!hasBaseData)
                {
                    _logger.LogInformation("Base tables are empty. Seeding Customers, Supermarkets, and Products...");
                    await ClearAllTablesAsync(ct);
                    await SeedBaseEntitiesAsync(seedDataDirectory, ct);
                }

                var hasOrders = await _dbContext.Orders.AnyAsync(ct) || await _dbContext.OrderDetails.AnyAsync(ct);
                if (!hasOrders)
                {
                    _logger.LogInformation("Order tables are empty. Seeding Orders and OrderDetails...");
                    await SeedOrdersAsync(ct);
                }

                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }, cancellationToken);
    }

    public async Task ForceReseedAsync(string seedDataDirectory, CancellationToken cancellationToken = default)
    {
        await ExecuteWithConcurrencyControlAsync(async ct =>
        {
            await _dbContext.Database.MigrateAsync(ct);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            try
            {
                await ClearAllTablesAsync(ct);
                await SeedBaseEntitiesAsync(seedDataDirectory, ct);
                await SeedOrdersAsync(ct);

                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }, cancellationToken);
    }

    private async Task ExecuteWithConcurrencyControlAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken)
    {
        await using var localLock = await _lockProvider.AcquireLockAsync(cancellationToken);

        var strategy = _dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            for (var attempt = 1; ; attempt++)
            {
                try
                {
                    await operation(cancellationToken);
                    return;
                }
                catch (Exception ex) when (attempt <= _options.MaxRetries && IsTransientOrLockException(ex))
                {
                    _logger.LogWarning(
                        ex,
                        "Database lock or concurrency conflict detected (attempt {Attempt}/{MaxRetries}). Retrying...",
                        attempt,
                        _options.MaxRetries);

                    _dbContext.ChangeTracker.Clear();
                    var delay = TimeSpan.FromMilliseconds(_options.RetryDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
                    await Task.Delay(delay, cancellationToken);
                }
            }
        });
    }

    private async Task ClearAllTablesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.OrderDetails.ExecuteDeleteAsync(cancellationToken);
        await _dbContext.Orders.ExecuteDeleteAsync(cancellationToken);
        await _dbContext.Products.ExecuteDeleteAsync(cancellationToken);
        await _dbContext.Customers.ExecuteDeleteAsync(cancellationToken);
        await _dbContext.Supermarkets.ExecuteDeleteAsync(cancellationToken);
    }

    private async Task SeedBaseEntitiesAsync(string seedDataDirectory, CancellationToken cancellationToken)
    {
        var customers = await SampleStreamAsync(
            _dataReader.ReadCustomersAsync(Path.Combine(seedDataDirectory, "customers_seed_data.txt"), cancellationToken),
            _options.CustomersToSeed,
            cancellationToken);

        var supermarkets = await SampleStreamAsync(
            _dataReader.ReadSupermarketsAsync(Path.Combine(seedDataDirectory, "supermarkets_seed_data.txt"), cancellationToken),
            _options.SupermarketsToSeed,
            cancellationToken);

        var products = await SampleStreamAsync(
            _dataReader.ReadProductsAsync(Path.Combine(seedDataDirectory, "products_seed_data.txt"), cancellationToken),
            _options.ProductsToSeed,
            cancellationToken);

        if (customers.Count < _options.CustomersToSeed ||
            supermarkets.Count < _options.SupermarketsToSeed ||
            products.Count < _options.ProductsToSeed)
        {
            throw new InvalidDataException("Seed files do not contain enough valid records.");
        }

        await _dbContext.Customers.AddRangeAsync(
            customers.Select(c => new Customer
            {
                FirstName = c.FirstName,
                LastName = c.LastName,
                Address = c.Address,
                Discount = c.Discount
            }),
            cancellationToken);

        await _dbContext.Supermarkets.AddRangeAsync(
            supermarkets.Select(s => new Supermarket
            {
                Name = s.Name,
                Address = s.Address
            }),
            cancellationToken);

        await _dbContext.Products.AddRangeAsync(
            products.Select(p => new Product
            {
                Name = p.Name,
                Price = p.Price
            }),
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedOrdersAsync(CancellationToken cancellationToken)
    {
        // Query actual generated keys rather than assuming 1..N identity sequences
        var customerIds = await _dbContext.Customers
            .Select(c => c.Id)
            .Take(_options.OrdersToSeed)
            .ToListAsync(cancellationToken);

        var supermarketIds = await _dbContext.Supermarkets
            .Select(s => s.Id)
            .Take(_options.OrdersToSeed)
            .ToListAsync(cancellationToken);

        var productIds = await _dbContext.Products
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        var orderCount = Math.Min(_options.OrdersToSeed, Math.Min(customerIds.Count, supermarketIds.Count));
        if (orderCount == 0 || productIds.Count == 0)
        {
            throw new InvalidOperationException("Cannot seed orders without existing Customers, Supermarkets, and Products.");
        }

        var orders = new List<Order>(orderCount);
        for (var i = 0; i < orderCount; i++)
        {
            var selectedProducts = productIds
                .OrderBy(_ => Random.Shared.Next())
                .Take(Math.Min(_options.ItemsPerOrder, productIds.Count));

            var order = new Order
            {
                CustomerId = customerIds[i],
                SupermarketId = supermarketIds[i],
                OrderDate = DateTime.UtcNow.AddDays(-(i + 1)),
                OrderDetails = selectedProducts.Select(pid => new OrderDetails
                {
                    ProductId = pid,
                    Quantity = _options.DefaultItemQuantity
                }).ToList()
            };

            orders.Add(order);
        }

        await _dbContext.Orders.AddRangeAsync(orders, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Algorithm R (Reservoir Sampling): selects k random records in O(N) time and O(k) bounded memory
    /// while still streaming and validating every line in the file.
    /// </summary>
    private static async Task<List<T>> SampleStreamAsync<T>(
        IAsyncEnumerable<T> source,
        int sampleSize,
        CancellationToken cancellationToken)
    {
        var reservoir = new List<T>(sampleSize);
        var index = 0;

        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            if (index < sampleSize)
            {
                reservoir.Add(item);
            }
            else
            {
                var j = Random.Shared.Next(index + 1);
                if (j < sampleSize)
                {
                    reservoir[j] = item;
                }
            }
            index++;
        }

        return reservoir;
    }

    private static bool IsTransientOrLockException(Exception ex)
    {
        if (ex is DbUpdateConcurrencyException)
            return true;

        var message = ex.ToString();
        return message.Contains("database is locked", StringComparison.OrdinalIgnoreCase)
            || message.Contains("deadlock", StringComparison.OrdinalIgnoreCase)
            || message.Contains("SQLITE_BUSY", StringComparison.OrdinalIgnoreCase);
    }
}



