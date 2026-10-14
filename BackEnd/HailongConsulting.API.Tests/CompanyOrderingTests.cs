using HailongConsulting.API.Common;
using HailongConsulting.API.Data;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Tests;

public class CompanyOrderingTests
{
    [Theory]
    [InlineData(false, "up")]
    [InlineData(false, "down")]
    [InlineData(true, "up")]
    [InlineData(true, "down")]
    public async Task MoveHandlesDuplicateOrderAndExcludesDeletedRows(bool scopes, string direction)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection).Options);
        await db.Database.EnsureCreatedAsync();
        if (scopes)
            db.BusinessScopes.AddRange(new BusinessScope { Id = 1, Name = "第一项", SortOrder = 0 }, new BusinessScope { Id = 2, Name = "第二项", SortOrder = 0 }, new BusinessScope { Id = 3, Name = "已删", SortOrder = 0, IsDeleted = 1 });
        else
            db.MajorAchievements.AddRange(new MajorAchievement { Id = 1, ProjectName = "第一项", SortOrder = 0 }, new MajorAchievement { Id = 2, ProjectName = "第二项", SortOrder = 0 }, new MajorAchievement { Id = 3, ProjectName = "已删", SortOrder = 0, IsDeleted = 1 });
        await db.SaveChangesAsync();
        var entities = db.ChangeTracker.Entries().Where(e => e.Entity is IVersionedContent).OrderBy(e => e.Property("Id").CurrentValue).ToList();
        var before = entities.Select(e => ((IVersionedContent)e.Entity).Version).ToArray();
        var source = direction == "up" ? 1 : 0;
        var dto = new MoveCompanyContentDto { Direction = direction, Version = before[source], NeighborId = source == 1 ? 1 : 2, NeighborVersion = before[1 - source] };
        var repository = new ConfigRepository(db);
        Assert.True(scopes ? await repository.MoveBusinessScopeAsync(source + 1, dto) : await repository.MoveAchievementAsync(source + 1, dto));
        Assert.Equal(2, entities[0].Property("SortOrder").CurrentValue);
        Assert.Equal(1, entities[1].Property("SortOrder").CurrentValue);
        Assert.NotEqual(before[0], ((IVersionedContent)entities[0].Entity).Version);
        Assert.NotEqual(before[1], ((IVersionedContent)entities[1].Entity).Version);
        Assert.Equal(before[2], ((IVersionedContent)entities[2].Entity).Version);
        Assert.Equal(0, entities[2].Property("SortOrder").CurrentValue);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task StaleVersionOrChangedNeighborRejectsWholeMove(bool scopes, bool changedNeighbor)
    {
        await using var db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        IVersionedContent first, second;
        if (scopes)
        {
            var a = new BusinessScope { Id = 1, Name = "第一项", SortOrder = 1 };
            var b = new BusinessScope { Id = 2, Name = "第二项", SortOrder = 2 };
            db.BusinessScopes.AddRange(a, b); first = a; second = b;
        }
        else
        {
            var a = new MajorAchievement { Id = 1, ProjectName = "第一项", SortOrder = 1 };
            var b = new MajorAchievement { Id = 2, ProjectName = "第二项", SortOrder = 2 };
            db.MajorAchievements.AddRange(a, b); first = a; second = b;
        }
        await db.SaveChangesAsync();
        var dto = new MoveCompanyContentDto { Direction = "down", Version = changedNeighbor ? first.Version : Guid.NewGuid().ToString("N"), NeighborId = changedNeighbor ? 3 : 2, NeighborVersion = second.Version };
        var repository = new ConfigRepository(db);
        await Assert.ThrowsAsync<ContentConflictException>(() => scopes ? repository.MoveBusinessScopeAsync(1, dto) : repository.MoveAchievementAsync(1, dto));
        Assert.Equal(1, db.Entry(first).Property("SortOrder").CurrentValue);
        Assert.Equal(2, db.Entry(second).Property("SortOrder").CurrentValue);
    }
}
