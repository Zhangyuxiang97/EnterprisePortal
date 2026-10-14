using System.Net;
using System.Net.Http.Json;
using AutoMapper;
using HailongConsulting.API.Common;
using HailongConsulting.API.Controllers;
using HailongConsulting.API.Data;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Repositories;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace HailongConsulting.API.Tests;

public class AnnouncementLinkTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private WebApplication _app = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddControllers().AddApplicationPart(typeof(HomeController).Assembly);
        builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(_connection));
        builder.Services.AddSingleton<IMapper>(new MapperConfiguration(c => c.AddProfile<MappingProfile>(), NullLoggerFactory.Instance).CreateMapper());
        builder.Services.AddSingleton<IHashIdService, HashIdService>();
        builder.Services.AddScoped<IHomeService, HomeService>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
        builder.Services.AddSingleton<IHtmlContentSanitizer, HtmlContentSanitizer>();
        builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
        builder.Services.AddScoped<IInfoPublicationRepository, InfoPublicationRepository>();
        builder.Services.AddScoped<IVisitStatisticRepository, VisitStatisticRepository>();
        builder.Services.AddScoped<IVisitStatisticService, VisitStatisticService>();
        _app = builder.Build();
        _app.MapControllers();
        await _app.StartAsync();
        _client = _app.GetTestClient();
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureCreatedAsync();
        db.Announcements.AddRange(
            Article(2299, "政府采购公告", "GOV_PROCUREMENT"),
            Article(2300, "建设工程公告", "CONSTRUCTION"),
            Article(2301, "停用公告", "GOV_PROCUREMENT", status: 0),
            Article(2302, "删除公告", "CONSTRUCTION", deleted: 1));
        await db.SaveChangesAsync();
    }

    private static Announcement Article(int id, string title, string business, sbyte status = 1, sbyte deleted = 0)
        => new() { Id = id, Title = title, Content = "<p>完整公告正文</p>", BusinessType = business,
            NoticeType = "bidding", Status = status, IsDeleted = deleted, PublishTime = new DateTime(2026, 10, 14) };

    [Fact]
    public async Task HomeAndListPublishTheSameWorkingHashLinksForBothBusinesses()
    {
        var home = (await _client.GetFromJsonAsync<ApiResponse<List<RecentAnnouncementDto>>>("/api/home/recent-announcements"))!.Data!;
        var list = (await _client.GetFromJsonAsync<ApiResponse<PagedResult<AnnouncementListDto>>>("/api/announcements"))!.Data!;
        Assert.Equal(2, home.Count);
        Assert.Contains(home, a => a.SourceType == "政府采购");
        Assert.Contains(home, a => a.SourceType == "建设工程");
        Assert.Equal("M0Ggj90m", home.Single(a => a.Id == 2299).HashId);
        foreach (var item in home)
        {
            Assert.Equal(list.Items.Single(a => a.Id == item.Id).HashId, item.HashId);
            var detail = await _client.GetFromJsonAsync<ApiResponse<AnnouncementDto>>($"/api/announcements/by-hash/{item.HashId}");
            Assert.True(detail!.Success);
            Assert.Equal(item.Id, detail.Data!.Id);
            Assert.Equal(item.Title, detail.Data.Title);
        }
    }

    [Fact]
    public async Task LegacyNumericLinkLoadsTheSameContentAndCountsEachRequestOnce()
    {
        foreach (var key in new[] { "2299", "M0Ggj90m" })
        {
            var response = await _client.GetFromJsonAsync<ApiResponse<AnnouncementDto>>($"/api/announcements/by-hash/{key}");
            Assert.True(response!.Success);
            Assert.Equal(2299, response.Data!.Id);
            Assert.Equal("M0Ggj90m", response.Data.HashId);
            Assert.Equal("<p>完整公告正文</p>", response.Data.Content);
        }
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.Equal(2, (await db.Announcements.FindAsync(2299))!.ViewCount);
    }

    [Theory]
    [InlineData("2301")]
    [InlineData("2302")]
    [InlineData("999999")]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("2147483648")]
    [InlineData("invalid-link")]
    public async Task LegacyLinksDoNotExposeHiddenOrMissingContent(string key)
    {
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"/api/announcements/by-hash/{key}")).StatusCode);
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.All(await db.Announcements.ToListAsync(), a => Assert.Equal(0, a.ViewCount));
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _app.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
