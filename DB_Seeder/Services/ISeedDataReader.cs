using DB_Seeder.Models;

namespace DB_Seeder.Services;

public interface ISeedDataReader
{
    IAsyncEnumerable<CustomerSeed> ReadCustomersAsync(string filePath, CancellationToken cancellationToken = default);
    IAsyncEnumerable<SupermarketSeed> ReadSupermarketsAsync(string filePath, CancellationToken cancellationToken = default);
    IAsyncEnumerable<ProductSeed> ReadProductsAsync(string filePath, CancellationToken cancellationToken = default);
}



