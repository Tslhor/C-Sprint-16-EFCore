using System.Globalization;
using System.Runtime.CompilerServices;
using CsvHelper;
using CsvHelper.Configuration;
using DB_Seeder.Models;
using DB_Seeder.Validation;

namespace DB_Seeder.Services;

/// <summary>
/// Streams and parses CSV/comma-delimited seed files safely with CsvHelper,
/// properly handling quoted fields containing commas and validating business rules.
/// </summary>
public sealed class CsvSeedDataReader : ISeedDataReader
{
    private readonly ISeedDataValidator _validator;

    public CsvSeedDataReader(ISeedDataValidator validator)
    {
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    private static CsvConfiguration CreateCsvConfig() => new(CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = false,
        TrimOptions = TrimOptions.Trim,
        IgnoreBlankLines = true,
        BadDataFound = context =>
            throw new InvalidDataException($"Malformed CSV data on row {context.Context?.Parser?.Row ?? 0}: '{context.RawRecord ?? string.Empty}'")
    };

    public async IAsyncEnumerable<CustomerSeed> ReadCustomersAsync(
        string filePath,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CreateCsvConfig());

        while (await csv.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var rowNumber = csv.Parser.Row;

            if (csv.Parser.Count != 4 ||
                !csv.TryGetField<string>(0, out var firstName) ||
                !csv.TryGetField<string>(1, out var lastName) ||
                !csv.TryGetField<string>(2, out var address) ||
                !csv.TryGetField<int>(3, out var discount))
            {
                throw new InvalidDataException($"Invalid customer record format on line {rowNumber}.");
            }

            var record = new CustomerSeed(firstName!, lastName!, address!, discount);
            _validator.ValidateCustomer(record, rowNumber);
            yield return record;
        }
    }

    public async IAsyncEnumerable<SupermarketSeed> ReadSupermarketsAsync(
        string filePath,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CreateCsvConfig());

        while (await csv.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var rowNumber = csv.Parser.Row;

            if (csv.Parser.Count != 2 ||
                !csv.TryGetField<string>(0, out var name) ||
                !csv.TryGetField<string>(1, out var address))
            {
                throw new InvalidDataException($"Invalid supermarket record format on line {rowNumber}.");
            }

            var record = new SupermarketSeed(name!, address!);
            _validator.ValidateSupermarket(record, rowNumber);
            yield return record;
        }
    }

    public async IAsyncEnumerable<ProductSeed> ReadProductsAsync(
        string filePath,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CreateCsvConfig());

        while (await csv.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var rowNumber = csv.Parser.Row;

            if (csv.Parser.Count != 2 ||
                !csv.TryGetField<string>(0, out var name) ||
                !csv.TryGetField<decimal>(1, out var price))
            {
                throw new InvalidDataException($"Invalid product record format on line {rowNumber}.");
            }

            var record = new ProductSeed(name!, price);
            _validator.ValidateProduct(record, rowNumber);
            yield return record;
        }
    }
}



