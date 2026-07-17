using System.Reflection;
using HailongConsulting.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Services;

/// <summary>
/// 执行内嵌且版本化的 SQL 迁移。每个脚本只会在 schema_migrations 中登记一次。
/// </summary>
public static class DatabaseMigrationRunner
{
    public static async Task ApplyDatabaseMigrationsAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var connection = dbContext.Database.GetDbConnection();

        await connection.OpenAsync();
        try
        {
            await using (var command = connection.CreateCommand())
            {
                command.CommandText = """
                    CREATE TABLE IF NOT EXISTS schema_migrations (
                        version VARCHAR(160) NOT NULL PRIMARY KEY,
                        applied_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
                    """;
                await command.ExecuteNonQueryAsync();
            }

            var assembly = typeof(DatabaseMigrationRunner).Assembly;
            var migrations = assembly.GetManifestResourceNames()
                .Where(name => name.Contains(".DatabaseMigrations.", StringComparison.Ordinal) && name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                .OrderBy(name => name, StringComparer.Ordinal);

            foreach (var resourceName in migrations)
            {
                var version = resourceName.Split(".DatabaseMigrations.", StringSplitOptions.None)[1][..^4];
                await using var existsCommand = connection.CreateCommand();
                existsCommand.CommandText = "SELECT 1 FROM schema_migrations WHERE version = @version LIMIT 1";
                var versionParameter = existsCommand.CreateParameter();
                versionParameter.ParameterName = "@version";
                versionParameter.Value = version;
                existsCommand.Parameters.Add(versionParameter);
                if (await existsCommand.ExecuteScalarAsync() is not null)
                {
                    continue;
                }

                await using var stream = assembly.GetManifestResourceStream(resourceName)
                    ?? throw new InvalidOperationException($"无法读取数据库迁移资源: {resourceName}");
                using var reader = new StreamReader(stream);
                var sql = await reader.ReadToEndAsync();

                await using var transaction = await connection.BeginTransactionAsync();
                try
                {
                    await using (var migrationCommand = connection.CreateCommand())
                    {
                        migrationCommand.Transaction = transaction;
                        migrationCommand.CommandText = sql;
                        await migrationCommand.ExecuteNonQueryAsync();
                    }

                    await using (var recordCommand = connection.CreateCommand())
                    {
                        recordCommand.Transaction = transaction;
                        recordCommand.CommandText = "INSERT INTO schema_migrations (version) VALUES (@version)";
                        var parameter = recordCommand.CreateParameter();
                        parameter.ParameterName = "@version";
                        parameter.Value = version;
                        recordCommand.Parameters.Add(parameter);
                        await recordCommand.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}
