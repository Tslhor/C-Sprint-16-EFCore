namespace DB_Seeder.Models;

public sealed record CustomerSeed(
    string FirstName,
    string LastName,
    string Address,
    int Discount);

public sealed record SupermarketSeed(
    string Name,
    string Address);

public sealed record ProductSeed(
    string Name,
    decimal Price);



