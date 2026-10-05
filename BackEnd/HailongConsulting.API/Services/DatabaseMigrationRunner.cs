using System.Reflection;
using HailongConsulting.API.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;

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
        // 迁移中的 SET/PREPARE 使用会话变量。独立连接避免改变业务连接的选项。
        var settings = new MySqlConnectionStringBuilder(dbContext.Database.GetConnectionString()
            ?? throw new InvalidOperationException("数据库连接未配置")) { AllowUserVariables = true };
        await using var connection = new MySqlConnection(settings.ConnectionString);

        await connection.OpenAsync();
        var locked = false;
        try
        {
            await using (var acquire = connection.CreateCommand())
            {
                acquire.CommandTimeout = 70;
                acquire.CommandText = "SELECT GET_LOCK(SHA2(CONCAT(DATABASE(), ':hailong-migrations'), 256), 60)";
                locked = Convert.ToInt32(await acquire.ExecuteScalarAsync()) == 1;
                if (!locked) throw new InvalidOperationException("其他实例正在升级数据库，请稍后重新启动。");
            }
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

                // MySQL DDL 隐式提交：采用串行锁和可重入脚本，全部成功后才登记版本。
                await using (var migrationCommand = connection.CreateCommand())
                {
                    migrationCommand.CommandTimeout = 300;
                    migrationCommand.CommandText = sql;
                    await migrationCommand.ExecuteNonQueryAsync();
                }

                await using (var recordCommand = connection.CreateCommand())
                {
                    recordCommand.CommandText = "INSERT INTO schema_migrations (version) VALUES (@version)";
                    recordCommand.Parameters.AddWithValue("@version", version);
                    await recordCommand.ExecuteNonQueryAsync();
                }
            }
        }
        finally
        {
            if (locked && connection.State == ConnectionState.Open)
            {
                try
                {
                    await using var release = connection.CreateCommand();
                    release.CommandText = "SELECT RELEASE_LOCK(SHA2(CONCAT(DATABASE(), ':hailong-migrations'), 256))";
                    await release.ExecuteScalarAsync();
                }
                catch { /* 关闭连接也会释放锁，不覆盖原始迁移异常。 */ }
            }
            await connection.CloseAsync();
        }
    }
}
