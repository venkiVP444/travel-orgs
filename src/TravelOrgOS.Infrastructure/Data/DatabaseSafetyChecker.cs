using Microsoft.Data.SqlClient;

namespace TravelOrgOS.Infrastructure.Data;

public static class DatabaseSafetyChecker
{
    public const string AllowedServer = @"(localdb)\MSSQLLocalDB";
    public const string AllowedDatabase = "TravelOrgOS_Dev";

    public static void AssertConnectionIsLocalDbOnly(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("CRITICAL SAFETY ERROR: Connection string is empty!");
        }

        if (connectionString.Contains("10.50.6.6", StringComparison.OrdinalIgnoreCase) ||
            connectionString.Contains("dbEMMA_Restore", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("CRITICAL SAFETY VIOLATION: Accessing office database (10.50.6.6 / dbEMMA_Restore) is strictly forbidden!");
        }

        var builder = new SqlConnectionStringBuilder(connectionString);

        var dataSource = builder.DataSource?.Trim();
        var database = builder.InitialCatalog?.Trim();

        if (!string.Equals(dataSource, AllowedServer, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"CRITICAL SAFETY VIOLATION: Data Source '{dataSource}' is not allowed! Must be '{AllowedServer}'.");
        }

        if (!string.Equals(database, AllowedDatabase, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"CRITICAL SAFETY VIOLATION: Database '{database}' is not allowed! Must be '{AllowedDatabase}'.");
        }
    }
}
