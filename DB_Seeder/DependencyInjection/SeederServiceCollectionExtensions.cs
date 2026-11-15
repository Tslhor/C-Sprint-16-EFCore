using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Data;
using DB_Seeder.Concurrency;
using DB_Seeder.Services;
using DB_Seeder.Validation;

namespace DB_Seeder.DependencyInjection;

public static class SeederServiceCollectionExtensions
{
    public static IServiceCollection AddShoppingSqliteSeeder(
        this IServiceCollection services,
        string sqliteConnectionString,
        DatabaseSeederOptions? options = null)
    {
        services.AddDbContext<ShoppingDbContext>(db =>
                db.UseSqlite(sqliteConnectionString, x => x.MigrationsAssembly(typeof(ShoppingDbContext).Assembly.GetName().Name)));

        RegisterSeederServices(services, options);
        return services;
    }

    public static IServiceCollection AddShoppingSqlServerSeeder(
        this IServiceCollection services,
        string sqlServerConnectionString,
        DatabaseSeederOptions? options = null)
    {
        services.AddDbContext<ShoppingDbContext>(db =>
            db.UseSqlServer(sqlServerConnectionString, x =>
            {
                x.MigrationsAssembly(typeof(ShoppingDbContext).Assembly.GetName().Name);
                x.EnableRetryOnFailure();
            }));

        RegisterSeederServices(services, options);
        return services;
    }

    private static void RegisterSeederServices(IServiceCollection services, DatabaseSeederOptions? options)
    {
        services.AddSingleton(options ?? new DatabaseSeederOptions());
        services.AddSingleton<ISeedLockProvider, InProcessSeedLockProvider>();
        services.AddSingleton<ISeedDataValidator, SeedDataValidator>();
        services.AddScoped<ISeedDataReader, CsvSeedDataReader>();
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
    }
}



