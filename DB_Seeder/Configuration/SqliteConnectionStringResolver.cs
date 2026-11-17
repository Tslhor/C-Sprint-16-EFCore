using Microsoft.Data.Sqlite;

namespace DB_Seeder.Configuration;

public static class SqliteConnectionStringResolver
{
    [Obsolete("Use Resolve(connectionString, contentRootPath).")]
    public static string ResolveAndPrepare(string connectionString, string contentRootPath)
        => Resolve(connectionString, contentRootPath);

    public static string Resolve(string connectionString, string contentRootPath)
    {
        var sqliteBuilder = new SqliteConnectionStringBuilder(connectionString);
        var dataSource = sqliteBuilder.DataSource;

        if (!string.IsNullOrWhiteSpace(dataSource)
            && !string.Equals(dataSource, ":memory:", StringComparison.OrdinalIgnoreCase)
            && !dataSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase)
            && !Path.IsPathRooted(dataSource))
        {
            var anchorPath = ResolveProjectAnchorPath(contentRootPath);
            sqliteBuilder.DataSource = Path.GetFullPath(Path.Combine(anchorPath, dataSource));
        }

        return sqliteBuilder.ToString();
    }

    private static string ResolveProjectAnchorPath(string contentRootPath)
    {
        if (ContainsProjectFile(contentRootPath))
        {
            return contentRootPath;
        }

        var fromContentRoot = FindNearestProjectDirectory(contentRootPath);
        if (!string.IsNullOrWhiteSpace(fromContentRoot))
        {
            return fromContentRoot;
        }

        var fromBaseDirectory = FindNearestProjectDirectory(AppContext.BaseDirectory);
        if (!string.IsNullOrWhiteSpace(fromBaseDirectory))
        {
            return fromBaseDirectory;
        }

        return contentRootPath;
    }

    private static string? FindNearestProjectDirectory(string startPath)
    {
        if (string.IsNullOrWhiteSpace(startPath))
        {
            return null;
        }

        var current = new DirectoryInfo(Path.GetFullPath(startPath));
        while (current is not null)
        {
            if (ContainsProjectFile(current.FullName))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        return null;
    }

    private static bool ContainsProjectFile(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
        {
            return false;
        }

        return Directory.EnumerateFiles(directoryPath, "*.csproj", SearchOption.TopDirectoryOnly).Any();
    }
}
