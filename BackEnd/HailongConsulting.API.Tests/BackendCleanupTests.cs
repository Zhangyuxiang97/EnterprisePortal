using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Text.Json;
using AutoMapper;
using HailongConsulting.API.Common;
using HailongConsulting.API.Controllers;
using HailongConsulting.API.Data;
using HailongConsulting.API.Middleware;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Repositories;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;

namespace HailongConsulting.API.Tests;

public class BackendCleanupTests
{
    private static IMapper Mapper() => new MapperConfiguration(c => c.AddProfile<MappingProfile>(), NullLoggerFactory.Instance).CreateMapper();
    private static AnnouncementService Announcements(ApplicationDbContext db) => new(new UnitOfWork(db), Mapper(), NullLogger<AnnouncementService>.Instance, new HtmlContentSanitizer());
    private static InfoPublicationService Publications(ApplicationDbContext db) => new(new UnitOfWork(db), Mapper(), NullLogger<InfoPublicationService>.Instance, new HtmlContentSanitizer());
    private static UserService Users(ApplicationDbContext db) => new(new UnitOfWork(db), db, new UserRepository(db), Mapper(), NullLogger<UserService>.Instance);
    private static Announcement Article(string title, int status = 1) => new() {
        Title = title, Content = "<p>正文</p>", BusinessType = "GOV_PROCUREMENT", NoticeType = "result", ProcurementType = "service", Status = (sbyte)status
    };

    private sealed class Database : IAsyncDisposable
    {
        public SqliteConnection Connection { get; } = new("Data Source=:memory:");
        public Recorder Queries { get; } = new();
        public ApplicationDbContext Open() => new(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(Connection).AddInterceptors(Queries).Options);
        public async Task Initialize() { await Connection.OpenAsync(); await using var db = Open(); await db.Database.EnsureCreatedAsync(); }
        public ValueTask DisposeAsync() => Connection.DisposeAsync();
    }
    private sealed class Recorder : DbCommandInterceptor
    {
        public List<string> Commands { get; } = [];
        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData,
            InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        { Commands.Add(command.CommandText); return ValueTask.FromResult(result); }
    }

    [Fact]
    public async Task PublicReadsHideDisabledContentAndManagementReadsDoNotCountAsViews()
    {
        await using var database = new Database(); await database.Initialize(); await using var db = database.Open();
        db.Announcements.AddRange(Article("公开"), Article("停用", 0));
        db.InfoPublications.AddRange(new InfoPublication { Title = "公开资讯", Content = "正文", Type = "COMPANY_NEWS" },
            new InfoPublication { Title = "停用资讯", Content = "正文", Type = "COMPANY_NEWS", Status = 0 });
        await db.SaveChangesAsync();
        var announcements = Announcements(db); var publications = Publications(db);
        Assert.Equal(1, (await announcements.GetPagedAsync(new())).TotalCount);
        Assert.Equal(2, (await announcements.GetPagedAsync(new(), true)).TotalCount);
        Assert.Null(await announcements.GetByIdAsync(2)); Assert.NotNull(await announcements.GetByIdAsync(2, true));
        Assert.Null(await publications.GetByIdAsync(2)); Assert.NotNull(await publications.GetByIdAsync(2, true));
        await announcements.GetByIdAsync(1, true); await publications.GetByIdAsync(1, true);
        Assert.Equal(0, (await db.Announcements.FindAsync(1))!.ViewCount);
        Assert.Equal(0, (await db.InfoPublications.FindAsync(1))!.ViewCount);
        Assert.Equal(1, (await publications.GetPagedForPortalAsync(new())).TotalCount);
        Assert.Equal(2, (await publications.GetPagedAsync(new())).TotalCount);
        Assert.Single(await new HomeService(db).GetRecentAnnouncementsAsync());
    }

    [Fact]
    public async Task ListQueriesOmitBodiesAndResolveRegionsOnceForTheWholePage()
    {
        await using var database = new Database(); await database.Initialize(); await using var db = database.Open();
        db.RegionDictionaries.Add(new RegionDictionary { RegionCode = "410000", RegionName = "河南省", RegionLevel = 1 });
        for (var i = 0; i < 20; i++) { var item = Article($"公告{i}"); item.Province = "410000"; db.Announcements.Add(item); }
        await db.SaveChangesAsync(); db.ChangeTracker.Clear(); database.Queries.Commands.Clear();
        var list = await Announcements(db).GetPagedAsync(new() { PageSize = 20 }, true);
        Assert.Equal(20, list.Items.Count()); Assert.All(list.Items, a => Assert.Equal("河南省", a.Province));
        Assert.All(database.Queries.Commands, sql => Assert.DoesNotContain("\"content\"", sql));
        Assert.Single(database.Queries.Commands, sql => sql.Contains("FROM \"region_dictionary\""));
        Assert.Empty(db.ChangeTracker.Entries());
        Assert.DoesNotContain("\"content\":", JsonSerializer.Serialize(list, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
    }

    [Fact]
    public async Task ContentVersionRejectsLostUpdatesWhileViewsRemainIndependent()
    {
        await using var database = new Database(); await database.Initialize();
        await using (var seed = database.Open()) { seed.Announcements.Add(Article("初始")); await seed.SaveChangesAsync(); }
        await using var editor = database.Open(); await using var other = database.Open();
        var loaded = await editor.Announcements.SingleAsync(); var version = loaded.Version;
        await new AnnouncementRepository(other).IncrementViewCountAsync(loaded.Id);
        var saved = await Announcements(editor).UpdateAsync(loaded.Id, new UpdateAnnouncementDto { Version = version, Title = "甲保存" });
        Assert.NotEqual(version, saved!.Version);
        await Assert.ThrowsAsync<ContentConflictException>(() => Announcements(other).UpdateAsync(loaded.Id, new UpdateAnnouncementDto { Version = version, Title = "乙过期" }));
        await using var check = database.Open(); var actual = await check.Announcements.SingleAsync();
        Assert.Equal("甲保存", actual.Title); Assert.Equal(1, actual.ViewCount);
        var beforeView = actual.Version; await new AnnouncementRepository(check).IncrementViewCountAsync(actual.Id);
        check.ChangeTracker.Clear(); Assert.Equal(beforeView, (await check.Announcements.SingleAsync()).Version);
    }

    [Fact]
    public async Task DatabaseConcurrencyTokenStopsTwoEditorsThatLoadedTheSameVersion()
    {
        await using var database = new Database(); await database.Initialize();
        await using var first = database.Open(); first.Announcements.Add(Article("原文")); await first.SaveChangesAsync();
        await using var second = database.Open(); var stale = await second.Announcements.SingleAsync();
        var current = await first.Announcements.SingleAsync(); current.Title = "先保存"; await first.SaveChangesAsync();
        stale.Title = "后保存";
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => second.SaveChangesAsync());
    }

    [Fact]
    public async Task UnchangedCompanyDetailsSaveSuccessfullyWithoutRotatingTheVersion()
    {
        await using var database = new Database(); await database.Initialize(); await using var db = database.Open();
        var attachmentService = new AttachmentService(new UnitOfWork(db), db, Mapper(), NullLogger<AttachmentService>.Instance);
        var service = new ConfigService(new ConfigRepository(db), Mapper(), attachmentService, new HtmlContentSanitizer());
        var item = await service.CreateQualificationAsync(new CreateCompanyQualificationDto { Name = "营业执照", Status = true });
        Assert.True(await service.UpdateQualificationAsync(item.Id, new UpdateCompanyQualificationDto { Name = item.Name, Version = item.Version, Status = item.Status }));
        Assert.Equal(item.Version, (await service.GetQualificationByIdAsync(item.Id))!.Version);
    }

    [Fact]
    public async Task LastActiveAdministratorCannotBeDeletedDisabledOrDemoted()
    {
        await using var database = new Database(); await database.Initialize(); await using var db = database.Open();
        db.Users.Add(new User { Username = "one", Email = "one@example.test", Role = "admin" }); await db.SaveChangesAsync();
        var service = Users(db); var id = (await db.Users.SingleAsync()).Id;
        await Assert.ThrowsAsync<ValidationException>(() => service.DeleteAsync(id));
        await Assert.ThrowsAsync<ValidationException>(() => service.ToggleStatusAsync(id));
        await Assert.ThrowsAsync<ValidationException>(() => service.UpdateAsync(id, new UpdateUserDto { Role = "user", Email = "one@example.test", RealName = "测试", Status = 1 }));
        Assert.True(Assert.Single((await service.GetPagedListAsync(new())).Items).IsLastActiveAdmin);
        db.Users.Add(new User { Username = "two", Email = "two@example.test", Role = "admin" }); await db.SaveChangesAsync();
        Assert.True(await service.DeleteAsync(id));
        Assert.Equal(1, await db.Users.CountAsync(u => u.Role == "admin" && u.Status == 1 && u.IsDeleted == 0));
    }

    [Fact]
    public async Task BatchAttachmentReferenceChecksUseAFixedQueryCountAndProtectAllSelectedFiles()
    {
        await using var database = new Database(); await database.Initialize(); await using var db = database.Open();
        for (var i = 1; i <= 3; i++) db.Attachments.Add(new Attachment { Id = i, FileName = $"{i}.jpg", FilePath = $"uploads/{i}.jpg", FileUrl = $"/uploads/{i}.jpg" });
        var article = Article("正文引用"); article.Content = "<img src='/uploads/1.jpg'>"; article.AttachmentIds = "[ 2 ]"; db.Announcements.Add(article);
        db.CompanyQualifications.Add(new CompanyQualification { Name = "资质", ImageId = 2 }); await db.SaveChangesAsync();
        database.Queries.Commands.Clear(); await AttachmentReferences.FindAsync(db, 1); var singleCount = database.Queries.Commands.Count;
        database.Queries.Commands.Clear(); var references = await AttachmentReferences.FindManyAsync(db, [1,2,3]);
        Assert.Equal(singleCount, database.Queries.Commands.Count); Assert.Equal(11, singleCount);
        Assert.Single(references[1]); Assert.Equal(2, references[2].Count); Assert.Empty(references[3]);
        var service = new AttachmentService(new UnitOfWork(db), db, Mapper(), NullLogger<AttachmentService>.Instance);
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteRangeAsync([3,2]));
        Assert.Equal(0, (await db.Attachments.FindAsync(3))!.IsDeleted);
    }

    [Theory]
    [InlineData("<p><br></p>")]
    [InlineData("<p>&nbsp;\u200b</p>")]
    [InlineData("<script>alert(1)</script>")]
    public async Task EmptyOrSanitizedAwayContentCannotBeSaved(string html)
    {
        await using var db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        await Assert.ThrowsAsync<ValidationException>(() => Publications(db).CreateAsync(new CreateInfoPublicationDto { Type = "COMPANY_NEWS", Title = "测试", Content = html }));
        Assert.Empty(await db.InfoPublications.ToListAsync());
    }

    [Fact]
    public void PaginationAndEnumValidationRejectInvalidInput()
    {
        static bool Valid(object dto) => Validator.TryValidateObject(dto, new ValidationContext(dto), [], true);
        Assert.False(Valid(new AnnouncementQueryDto { PageNumber = 0 }));
        Assert.False(Valid(new InfoPublicationQueryDto { PageSize = 101 }));
        Assert.False(Valid(new UserQueryDto { Page = 0 }));
        Assert.False(Valid(new CreateInfoPublicationDto { Type = "unknown", Title = "标题", Content = "正文" }));
        Assert.True(ContentValidation.HasContent("<img src='/uploads/scan.jpg'>"));
    }

    [Fact]
    public async Task UnexpectedErrorsReturnTraceIdWithoutInternalDetails()
    {
        var context = new DefaultHttpContext { TraceIdentifier = "test-trace" }; context.Response.Body = new MemoryStream();
        var middleware = new ExceptionHandlingMiddleware(_ => throw new InvalidOperationException("internal-password-and-sql"), NullLogger<ExceptionHandlingMiddleware>.Instance);
        await middleware.InvokeAsync(context);
        context.Response.Body.Position = 0; var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Equal(500, context.Response.StatusCode); Assert.Contains("test-trace", body); Assert.DoesNotContain("internal-password-and-sql", body);
    }
}
