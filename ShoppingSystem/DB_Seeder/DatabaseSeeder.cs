using System.Globalization;
using Microsoft.Data.Sqlite;

namespace ShoppingSystem.DB_Seeder;

public static class DatabaseSeeder
{
    public static async Task SeedIfEmptyAsync(string databasePath, string seedDataPath)
    {
        if (!File.Exists(databasePath))
        {
            await SeedAsync(databasePath, seedDataPath);
        }
        else
        {
            await EnsureSchemaAsync(databasePath);

            if (await IsBaseDataEmptyAsync(databasePath))
            {
                await SeedAsync(databasePath, seedDataPath);
            }
        }

        if (await IsOrderDataEmptyAsync(databasePath))
        {
            await SeedOrdersAsync(databasePath);
        }
    }

    public static async Task SeedAsync(string databasePath, string seedDataPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);

        await EnsureSchemaAsync(databasePath);

        await using var connection = new SqliteConnection($"Data Source={databasePath}");
        await connection.OpenAsync();

        var customers = ReadCustomers(
            Path.Combine(seedDataPath, "customers_seed_data.txt"));
        var supermarkets = ReadSupermarkets(
            Path.Combine(seedDataPath, "supermarkets_seed_data.txt"));
        var products = ReadProducts(
            Path.Combine(seedDataPath, "products_seed_data.txt"));

        if (customers.Count < 10 || supermarkets.Count < 5 || products.Count < 15)
        {
            throw new InvalidDataException("Seed files do not contain enough valid records.");
        }

        await using var transaction = (SqliteTransaction)await connection.BeginTransactionAsync();

        var tables = new[]
        {
            "OrderDetails",
            "Orders",
            "Products",
            "Customers",
            "SuperMarkets"
        };

        foreach (var table in tables)
        {
            await ExecuteAsync(connection, transaction, $"DELETE FROM {table};");
        }

        foreach (var customer in customers
                     .OrderBy(_ => Random.Shared.Next())
                     .Take(10))
        {
            const string sql = """
                INSERT INTO Customers (FirstName, LastName, Address, Discount)
                VALUES ($firstName, $lastName, $address, $discount);
                """;

            await InsertAsync(
                connection,
                transaction,
                sql,
                ("$firstName", customer.FirstName),
                ("$lastName", customer.LastName),
                ("$address", customer.Address),
                ("$discount", customer.Discount));
        }

        foreach (var supermarket in supermarkets
                     .OrderBy(_ => Random.Shared.Next())
                     .Take(5))
        {
            const string sql = """
                INSERT INTO SuperMarkets (Name, Address)
                VALUES ($name, $address);
                """;

            await InsertAsync(
                connection,
                transaction,
                sql,
                ("$name", supermarket.Name),
                ("$address", supermarket.Address));
        }

        foreach (var product in products
                     .OrderBy(_ => Random.Shared.Next())
                     .Take(15))
        {
            const string sql = """
                INSERT INTO Products (Name, Price)
                VALUES ($name, $price);
                """;
            var price = product.Price.ToString("0.00", CultureInfo.InvariantCulture);

            await InsertAsync(
                connection,
                transaction,
                sql,
                ("$name", product.Name),
                ("$price", price));
        }

        await transaction.CommitAsync();
    }

    private static async Task EnsureSchemaAsync(string databasePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);

        await using var connection = new SqliteConnection($"Data Source={databasePath}");
        await connection.OpenAsync();

        await using var schemaCommand = connection.CreateCommand();
        var schemaPath = Path.Combine(Path.GetDirectoryName(databasePath)!, "schema.sql");
        schemaCommand.CommandText = await File.ReadAllTextAsync(schemaPath);
        await schemaCommand.ExecuteNonQueryAsync();
    }

    private static async Task SeedOrdersAsync(string databasePath)
    {
        await using var connection = new SqliteConnection($"Data Source={databasePath}");
        await connection.OpenAsync();
        await using var transaction = (SqliteTransaction)await connection.BeginTransactionAsync();

        for (var orderNumber = 1; orderNumber <= 5; orderNumber++)
        {
            const string orderSql = """
                INSERT INTO Orders (CustomerId, SupermarketId, OrderDate)
                VALUES ($customerId, $supermarketId, $orderDate);
                """;

            var orderDate = DateTime.UtcNow
                .AddDays(-orderNumber)
                .ToString("O", CultureInfo.InvariantCulture);

            var orderId = await InsertAndGetIdAsync(
                connection,
                transaction,
                orderSql,
                ("$customerId", orderNumber),
                ("$supermarketId", orderNumber),
                ("$orderDate", orderDate));

            const string detailSql = """
                INSERT INTO OrderDetails (OrderId, ProductId, Quantity)
                VALUES ($orderId, $productId, $quantity);
                """;

            foreach (var productId in Enumerable.Range(1, 15)
                         .OrderBy(_ => Random.Shared.Next())
                         .Take(6))
            {
                await InsertAsync(
                    connection,
                    transaction,
                    detailSql,
                    ("$orderId", orderId),
                    ("$productId", productId),
                    ("$quantity", "3.000"));
            }
        }

        await transaction.CommitAsync();
    }

    private static async Task<bool> IsBaseDataEmptyAsync(string databasePath)
    {
        await using var connection = new SqliteConnection($"Data Source={databasePath}");
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT CASE
                WHEN NOT EXISTS (SELECT 1 FROM Products) THEN 1
                ELSE 0
            END;
            """;

        return Convert.ToInt32(await command.ExecuteScalarAsync()) == 1;
    }

    private static async Task<bool> IsOrderDataEmptyAsync(string databasePath)
    {
        await using var connection = new SqliteConnection($"Data Source={databasePath}");
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT CASE
                WHEN NOT EXISTS (SELECT 1 FROM Orders)
                 AND NOT EXISTS (SELECT 1 FROM OrderDetails) THEN 1
                ELSE 0
            END;
            """;

        return Convert.ToInt32(await command.ExecuteScalarAsync()) == 1;
    }

    private static List<CustomerSeed> ReadCustomers(string path)
    {
        return File.ReadLines(path)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select((line, index) =>
            {
                var fields = line.Split(',', 4, StringSplitOptions.TrimEntries);
                if (fields.Length != 4 || !int.TryParse(fields[3], out var discount))
                {
                    throw new InvalidDataException($"Invalid customer record on line {index + 1}.");
                }

                return new CustomerSeed(fields[0], fields[1], fields[2], discount);
            })
            .ToList();
    }

    private static List<SupermarketSeed> ReadSupermarkets(string path)
    {
        return File.ReadLines(path)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select((line, index) =>
            {
                var fields = line.Split(',', 2, StringSplitOptions.TrimEntries);
                if (fields.Length != 2)
                {
                    throw new InvalidDataException($"Invalid supermarket record on line {index + 1}.");
                }

                return new SupermarketSeed(fields[0], fields[1]);
            })
            .ToList();
    }

    private static List<ProductSeed> ReadProducts(string path)
    {
        return File.ReadLines(path)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select((line, index) =>
            {
                var fields = line.Split(',', 2, StringSplitOptions.TrimEntries);
                if (fields.Length != 2 ||
                    !decimal.TryParse(fields[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var price))
                {
                    throw new InvalidDataException($"Invalid product record on line {index + 1}.");
                }

                return new ProductSeed(fields[0], price);
            })
            .ToList();
    }

    private static async Task ExecuteAsync(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string sql)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        await command.ExecuteNonQueryAsync();
    }

    private static async Task InsertAsync(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string sql,
        params (string Name, object Value)[] values)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;

        foreach (var value in values)
        {
            command.Parameters.AddWithValue(value.Name, value.Value);
        }

        await command.ExecuteNonQueryAsync();
    }

    private static async Task<long> InsertAndGetIdAsync(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string sql,
        params (string Name, object Value)[] values)
    {
        await InsertAsync(connection, transaction, sql, values);

        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT last_insert_rowid();";
        return Convert.ToInt64(await command.ExecuteScalarAsync());
    }

    private sealed record CustomerSeed(
        string FirstName,
        string LastName,
        string Address,
        int Discount);

    private sealed record SupermarketSeed(
        string Name, 
        string Address);

    private sealed record ProductSeed(
        string Name, 
        decimal Price);
}
