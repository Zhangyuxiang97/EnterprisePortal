using System.Data.Common;
using HailongConsulting.API.Controllers;
using HailongConsulting.API.Data;
using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;

namespace HailongConsulting.API.Tests;

public class VisitStatisticsTests
{
    [Theory]
    [InlineData(0, 20, null, null)]
    [InlineData(1, 101, null, null)]
    [InlineData(int.MaxValue, 100, null, null)]
    [InlineData(1, 20, "invalid", null)]
    [InlineData(1, 20, "2026-10-02", "2026-10-01")]
    public async Task InvalidPagingAndDatesReturnBadRequestBeforeQuery(int page, int pageSize, string? start, string? end)
    {
        var controller = new StatisticsController(null!, null!, null!, null!, null!, NullLogger<StatisticsController>.Instance);
        var result = await controller.GetVisitStatisticsList(page, pageSize, start, end, null);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task RepositoryFiltersCountsAndLimitsRowsInTheDatabaseWithStableOrdering()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var commands = new QueryRecorder();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection).AddInterceptors(commands).Options;
        await using var db = new ApplicationDbContext(options);
        // 只建本测试所需表，避免其他业务实体的 MySQL 专有 DDL。
        await db.Database.ExecuteSqlRawAsync("""
            CREATE TABLE visit_statistics (id INTEGER PRIMARY KEY, visit_date TEXT NOT NULL,
                page_url TEXT, page_title TEXT, visitor_ip TEXT, user_agent TEXT, referer TEXT,
                visit_count INTEGER NOT NULL, created_at TEXT NOT NULL, updated_at TEXT NOT NULL,
                is_deleted INTEGER NOT NULL);
            """);
        var date = new DateOnly(2026, 10, 1);
        for (var id = 1; id <= 15; id++)
            db.VisitStatistics.Add(new VisitStatistic { Id = id, VisitDate = date,
                PageUrl = id == 14 ? "/other" : "/announcement/1", IsDeleted = (sbyte)(id == 15 ? 1 : 0),
                CreatedAt = new DateTime(2026, 10, 1), UpdatedAt = new DateTime(2026, 10, 1), VisitCount = 1 });
        db.VisitStatistics.Add(new VisitStatistic { Id = 16, VisitDate = date.AddDays(-1), PageUrl = "/announcement/1", VisitCount = 1 });
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        commands.Sql.Clear();
        var repository = new VisitStatisticRepository(db);
        var result = await repository.GetPageAsync(date, date, "announcement", 2, 5);
        Assert.Equal(13, result.TotalCount);
        Assert.Equal(new[] { 8, 7, 6, 5, 4 }, result.Items.Select(x => x.Id));
        Assert.Empty(db.ChangeTracker.Entries());
        Assert.Contains(commands.Sql, sql => sql.Contains("COUNT(*)"));
        Assert.Contains(commands.Sql, sql => sql.Contains("LIMIT") && sql.Contains("OFFSET"));
        var beyond = await repository.GetPageAsync(date, date, "announcement", 10, 5);
        Assert.Empty(beyond.Items);
        Assert.Equal(13, beyond.TotalCount);
    }

    private sealed class QueryRecorder : DbCommandInterceptor
    {
        public List<string> Sql { get; } = [];
        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
            CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            Sql.Add(command.CommandText);
            return ValueTask.FromResult(result);
        }
    }
}
