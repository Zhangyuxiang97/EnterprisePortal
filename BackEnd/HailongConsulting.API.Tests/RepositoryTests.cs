using HailongConsulting.API.Data;
using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Repositories;
using HailongConsulting.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace HailongConsulting.API.Tests;

public class RepositoryTests
{
    [Fact]
    public async Task PortalVisitRepositoryDoesNotMergeVisitors()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new ConfigRepository(database.Context);
        var today = DateOnly.FromDateTime(DateTime.Now);

        await repository.RecordVisitAsync(new VisitStatistic
        {
            VisitDate = today, PageUrl = "/", VisitorIp = "192.0.2.1"
        });
        await repository.RecordVisitAsync(new VisitStatistic
        {
            VisitDate = today, PageUrl = "/", VisitorIp = "192.0.2.2"
        });

        Assert.Equal(2, await database.Context.VisitStatistics.CountAsync());
        Assert.Equal(2, await database.Context.VisitStatistics
            .Select(v => v.VisitorIp).Distinct().CountAsync());
    }

    [Fact]
    public async Task VisitsKeepEachVisitorAndWeightSourceCounts()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new VisitStatisticRepository(database.Context);

        await repository.RecordVisitAsync("/", "首页", "192.0.2.1", "browser", "search");
        await repository.RecordVisitAsync("/", "首页", "192.0.2.2", "browser", "direct");
        await database.Context.SaveChangesAsync();

        var service = new VisitStatisticService(
            repository, null!, null!, null!, database.Context,
            NullLogger<VisitStatisticService>.Instance);
        var today = DateOnly.FromDateTime(DateTime.Now);

        Assert.Equal(2, await service.GetUniqueVisitorsAsync(today, today));
        var visits = await repository.GetPagedByDateRangeAsync(today, today, "/", 1, 10);
        Assert.Equal(2, visits.TotalCount);
        Assert.Equal(2, visits.Items.Count());

        // 旧版已汇总的数据仍按 VisitCount 参与来源统计。
        database.Context.VisitStatistics.Add(new VisitStatistic
        {
            VisitDate = today,
            PageUrl = "/legacy",
            Referer = "search",
            VisitCount = 3
        });
        await database.Context.SaveChangesAsync();

        var sources = await service.GetVisitSourcesAsync();
        Assert.Equal(4, sources.Single(s => s.Source == "search").Count);
        Assert.Equal(1, sources.Single(s => s.Source == "direct").Count);
    }

    [Fact]
    public async Task AttachmentFiltersAndPaginationReturnExpectedPage()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new AttachmentRepository(database.Context);
        database.Context.Attachments.AddRange(
            NewAttachment("first.pdf", "document", 1),
            NewAttachment("second.pdf", "document", 2),
            NewAttachment("hidden.pdf", "document", 3, isDeleted: true),
            NewAttachment("photo.png", "image", 4));
        await database.Context.SaveChangesAsync();

        var result = await repository.GetFilteredPagedAsync(2, 1, "document", null, ".pdf");

        Assert.Equal(2, result.TotalCount);
        Assert.Equal("first.pdf", Assert.Single(result.Items).FileName);
    }

    private static Attachment NewAttachment(string name, string category, int minute, bool isDeleted = false) => new()
    {
        FileName = name,
        FilePath = $"/uploads/{name}",
        FileUrl = $"/uploads/{name}",
        Category = category,
        CreatedAt = new DateTime(2026, 1, 1, 0, minute, 0),
        IsDeleted = (sbyte)(isDeleted ? 1 : 0)
    };

    private sealed class TestDatabase : IAsyncDisposable
    {
        public ApplicationDbContext Context { get; }

        private TestDatabase(ApplicationDbContext context)
        {
            Context = context;
        }

        public static async Task<TestDatabase> CreateAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var context = new ApplicationDbContext(options);
            await context.Database.EnsureCreatedAsync();
            return new TestDatabase(context);
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
        }
    }
}
