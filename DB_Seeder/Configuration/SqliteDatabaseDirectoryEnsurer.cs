using Microsoft.Data.Sqlite;

namespace DB_Seeder.Configuration;

internal static class SqliteDatabaseDirectoryEnsurer
{
    public static void EnsureExists(string connectionString)
    {
        var sqliteBuilder = new SqliteConnectionStringBuilder(connectionString);
        var dbPath = sqliteBuilder.DataSource;

        if (string.IsNullOrWhiteSpace(dbPath)
            || string.Equals(dbPath, ":memory:", StringComparison.OrdinalIgnoreCase)
            || dbPath.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var dbDirectory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrWhiteSpace(dbDirectory))
        {
            Directory.CreateDirectory(dbDirectory);
        }
    }
}
