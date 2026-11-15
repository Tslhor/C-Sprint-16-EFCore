namespace DB_Seeder;

public interface IDatabaseSeeder
{
    Task SeedIfEmptyAsync(string seedDataDirectory, CancellationToken cancellationToken = default);
    Task ForceReseedAsync(string seedDataDirectory, CancellationToken cancellationToken = default);
}



