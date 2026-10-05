using System.ComponentModel.DataAnnotations;
using AutoMapper;
using HailongConsulting.API.Common;
using HailongConsulting.API.Data;
using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Repositories;
using HailongConsulting.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using MySqlConnector;

namespace HailongConsulting.API.Tests;

public sealed class LocalMySqlFactAttribute : FactAttribute
{
    public LocalMySqlFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("LOCAL_MYSQL_TEST_CONNECTION")))
            Skip = "需要显式提供本机隔离 MySQL 测试连接；测试创建并清理自己的随机数据库。";
    }
}

public class MySqlCleanupTests
{
    [LocalMySqlFact]
    public async Task CrossConnectionAdministratorLockBatchReferencesAndRestartMigrations()
    {
        var settings = new MySqlConnectionStringBuilder(Environment.GetEnvironmentVariable("LOCAL_MYSQL_TEST_CONNECTION")!);
        Assert.Contains(settings.Server, new[] { "127.0.0.1", "localhost" });
        var schema = "hailong_cleanup_test_" + Guid.NewGuid().ToString("N");
        settings.Database = "";
        await using var server = new MySqlConnection(settings.ConnectionString);
        await server.OpenAsync();
        await using (var create = server.CreateCommand()) { create.CommandText = $"CREATE DATABASE `{schema}` CHARACTER SET utf8mb4"; await create.ExecuteNonQueryAsync(); }
        settings.Database = schema;
        var connection = settings.ConnectionString;
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 44)), o => o.EnableRetryOnFailure()).Options;
        var mapper = new MapperConfiguration(c => c.AddProfile<MappingProfile>(), NullLoggerFactory.Instance).CreateMapper();
        try
        {
            await using (var seed = new ApplicationDbContext(options))
            {
                // Use the same schema as deployment, including MySQL defaults and full-text indexes.
                await using var schemaConnection = new MySqlConnection(connection);
                await schemaConnection.OpenAsync();
                await using var schemaCommand = schemaConnection.CreateCommand();
                schemaCommand.CommandTimeout = 180;
                schemaCommand.CommandText = (await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "TestSchema.sql"))).Replace("hailong_consulting", schema);
                await schemaCommand.ExecuteNonQueryAsync();
                // The reference checker also reads portal settings, introduced by a migration.
                var seedServices = new ServiceCollection().AddDbContext<ApplicationDbContext>(o => o.UseMySql(connection, new MySqlServerVersion(new Version(8,0,44))));
                await using var seedProvider = seedServices.BuildServiceProvider();
                await seedProvider.ApplyDatabaseMigrationsAsync();
                seed.Users.AddRange(new User { Id = 1, Username = "one", Email = "one@example.test", Role = "admin" }, new User { Id = 2, Username = "two", Email = "two@example.test", Role = "admin" });
                seed.Attachments.Add(new Attachment { Id = 1, FileName = "scan.jpg", FilePath = "uploads/scan.jpg", FileUrl = "/uploads/scan.jpg" });
                seed.InfoPublications.Add(new InfoPublication { Title = "图片引用", Content = "<img src='/uploads/scan.jpg'>", Type = "COMPANY_NEWS" });
                await seed.SaveChangesAsync();
                Assert.Single((await AttachmentReferences.FindManyAsync(seed, [1]))[1]);
            }

            // Another connection represents an independent application instance holding the admin rows.
            await using var independent = new MySqlConnection(connection); await independent.OpenAsync();
            await using var transaction = await independent.BeginTransactionAsync();
            await using (var locked = independent.CreateCommand())
            {
                locked.Transaction = transaction;
                locked.CommandText = "SELECT COUNT(*) FROM users WHERE role='admin' AND status=1 AND is_deleted=0 FOR UPDATE";
                Assert.Equal(2L, Convert.ToInt64(await locked.ExecuteScalarAsync()));
            }
            await using var editing = new ApplicationDbContext(options);
            var users = new UserService(new UnitOfWork(editing), editing, new UserRepository(editing), mapper, NullLogger<UserService>.Instance);
            var pending = users.DeleteAsync(2);
            await Task.WhenAny(pending, Task.Delay(200)); Assert.False(pending.IsCompleted);
            await using (var disable = independent.CreateCommand())
            {
                disable.Transaction = transaction; disable.CommandText = "UPDATE users SET status=0 WHERE id=1"; await disable.ExecuteNonQueryAsync();
            }
            await transaction.CommitAsync();
            await Assert.ThrowsAsync<ValidationException>(() => pending);
            Assert.Equal(1, await editing.Users.CountAsync(u => u.Role == "admin" && u.Status == 1 && u.IsDeleted == 0));

            // Run the actual embedded migrations twice against an existing schema.
            var services = new ServiceCollection().AddDbContext<ApplicationDbContext>(o => o.UseMySql(connection, new MySqlServerVersion(new Version(8,0,44))));
            await using var provider = services.BuildServiceProvider();
            await provider.ApplyDatabaseMigrationsAsync(); await provider.ApplyDatabaseMigrationsAsync();
            await using var verify = new MySqlConnection(connection); await verify.OpenAsync();
            await using var count = verify.CreateCommand(); count.CommandText = "SELECT COUNT(*) FROM schema_migrations";
            Assert.Equal(5L, Convert.ToInt64(await count.ExecuteScalarAsync()));
        }
        finally
        {
            MySqlConnection.ClearAllPools();
            // Only the fresh random schema created by this test is eligible for cleanup.
            Assert.StartsWith("hailong_cleanup_test_", schema);
            await using var drop = server.CreateCommand(); drop.CommandText = $"DROP DATABASE `{schema}`"; await drop.ExecuteNonQueryAsync();
        }
    }
}
