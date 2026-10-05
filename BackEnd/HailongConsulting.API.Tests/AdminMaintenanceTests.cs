using System.Data.Common;
using System.Text.Json;
using AutoMapper;
using HailongConsulting.API.Common;
using HailongConsulting.API.Data;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Repositories;
using HailongConsulting.API.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;

namespace HailongConsulting.API.Tests;

public class AdminMaintenanceTests
{
    private static IMapper Mapper() => new MapperConfiguration(c => c.AddProfile<MappingProfile>(), NullLoggerFactory.Instance).CreateMapper();
    private static ApplicationDbContext MemoryDb() => new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    private static InfoPublicationService Publications(ApplicationDbContext db) => new(new UnitOfWork(db), Mapper(), NullLogger<InfoPublicationService>.Instance, new HtmlContentSanitizer());
    private static AttachmentService Attachments(ApplicationDbContext db) => new(new UnitOfWork(db), db, Mapper(), NullLogger<AttachmentService>.Instance);

    [Fact]
    public async Task PolicyDocumentNumberPersistsAndExplicitNullClearsWithoutAffectingOtherPartialUpdates()
    {
        await using var db = MemoryDb();
        var service = Publications(db);
        var created = await service.CreateAsync(new CreateInfoPublicationDto {
            Type = "POLICY_REGULATION", Title = "政策测试", Content = "<p>政策正文</p>", DocumentNumber = "测试〔2026〕1号"
        });
        Assert.Equal("测试〔2026〕1号", (await service.GetByIdAsync(created.Id))!.DocumentNumber);
        await service.UpdateAsync(created.Id, new UpdateInfoPublicationDto { Version = (await db.Set<InfoPublication>().FindAsync(created.Id))!.Version, IsTop = true });
        Assert.Equal("测试〔2026〕1号", (await service.GetByIdAsync(created.Id))!.DocumentNumber);
        var clear = JsonSerializer.Deserialize<UpdateInfoPublicationDto>("{\"documentNumber\":null}", new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        clear.Version = (await db.Set<InfoPublication>().FindAsync(created.Id))!.Version;
        await service.UpdateAsync(created.Id, clear);
        db.ChangeTracker.Clear();
        Assert.Null((await service.GetByIdAsync(created.Id))!.DocumentNumber);
    }

    [Fact]
    public async Task RegionCannotDeleteChildrenOrAnnouncementReferencesAndDeletedCodesCanBeRecreated()
    {
        await using var db = MemoryDb();
        var service = new RegionDictionaryService(new RegionDictionaryRepository(db), new UnitOfWork(db), Mapper(), NullLogger<RegionDictionaryService>.Instance);
        var root = await service.CreateRegionAsync(new CreateRegionDictionaryDto { RegionCode = "990000", RegionName = "测试省", RegionLevel = 1 });
        var child = await service.CreateRegionAsync(new CreateRegionDictionaryDto { RegionCode = "990100", RegionName = "测试市", RegionLevel = 2, ParentCode = "990000" });
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteRegionAsync(root.Id));
        db.Announcements.Add(new Announcement { Title = "引用地区", Content = "正文", City = "990100" });
        await db.SaveChangesAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteRegionAsync(child.Id));
        db.Announcements.RemoveRange(db.Announcements);
        await db.SaveChangesAsync();
        await service.DeleteRegionAsync(child.Id);
        var restored = await service.CreateRegionAsync(new CreateRegionDictionaryDto { RegionCode = "990100", RegionName = "重新维护测试市", RegionLevel = 2, ParentCode = "990000" });
        Assert.Equal(child.Id, restored.Id);
        Assert.Equal("重新维护测试市", (await service.GetRegionByCodeAsync("990100"))!.RegionName);
    }

    [Fact]
    public async Task ClearingCoverAndAttachmentsDoesNotResurrectReferencesOnSave()
    {
        await using var db = MemoryDb();
        var service = Publications(db);
        var article = await service.CreateAsync(new CreateInfoPublicationDto {
            Type = "COMPANY_NEWS", Title = "附件解绑", Content = "<p>正文</p>", CoverImageId = 1, AttachmentIds = [2]
        });
        await service.UpdateAsync(article.Id, new UpdateInfoPublicationDto { Version = (await db.Set<InfoPublication>().FindAsync(article.Id))!.Version, IsTop = true });
        Assert.Equal(1, (await service.GetByIdAsync(article.Id))!.CoverImageId);
        await service.UpdateAsync(article.Id, new UpdateInfoPublicationDto { Version = (await db.Set<InfoPublication>().FindAsync(article.Id))!.Version, CoverImageId = null, AttachmentIds = [] });
        Assert.Null((await service.GetByIdAsync(article.Id))!.CoverImageId);
        Assert.Empty((await service.GetByIdAsync(article.Id))!.AttachmentIds ?? []);

        var announcements = new AnnouncementService(new UnitOfWork(db), Mapper(), NullLogger<AnnouncementService>.Instance, new HtmlContentSanitizer());
        var created = await announcements.CreateAsync(new CreateAnnouncementDto {
            ProcurementType = "service", Title = "清空公告附件", Content = "<p>正文</p>", BusinessType = "GOV_PROCUREMENT", NoticeType = "result",
            AttachmentIds = [2], Deadline = new DateTime(2026, 10, 1), Winner = "原单位"
        });
        await announcements.UpdateAsync(created.Id, new UpdateAnnouncementDto { Version = (await db.Set<Announcement>().FindAsync(created.Id))!.Version, AttachmentIds = [], Deadline = null, Winner = null });
        var saved = await announcements.GetByIdAsync(created.Id);
        Assert.Empty(saved!.AttachmentIds ?? []);
        Assert.Null(saved.Deadline);
        Assert.Null(saved.Winner);
    }

    [Fact]
    public async Task HistoricalCompanyFieldsCanBeUnknownAndClearedWhileOmittedFieldsRemain()
    {
        await using var db = MemoryDb();
        var service = new ConfigService(new ConfigRepository(db), Mapper(), Attachments(db), new HtmlContentSanitizer());
        var qualification = await service.CreateQualificationAsync(new CreateCompanyQualificationDto { Name = "营业执照", Status = true });
        await service.UpdateQualificationAsync(qualification.Id, new UpdateCompanyQualificationDto { Version = (await db.Set<CompanyQualification>().FindAsync(qualification.Id))!.Version, Name = "营业执照复核", IssueDate = new(2020, 1, 1), ExpiryDate = new(2025, 1, 1) });
        await service.UpdateQualificationAsync(qualification.Id, new UpdateCompanyQualificationDto { Version = (await db.Set<CompanyQualification>().FindAsync(qualification.Id))!.Version, Description = "已复核" });
        Assert.Equal(new DateOnly(2020, 1, 1), (await service.GetQualificationByIdAsync(qualification.Id))!.IssueDate);
        await service.UpdateQualificationAsync(qualification.Id, new UpdateCompanyQualificationDto { Version = (await db.Set<CompanyQualification>().FindAsync(qualification.Id))!.Version, IssueDate = null, ExpiryDate = null, CertificateImageId = null });
        var saved = await service.GetQualificationByIdAsync(qualification.Id);
        Assert.Null(saved!.IssueDate);
        Assert.Null(saved.ExpiryDate);
        Assert.True(saved.Status);
        var honor = await service.CreateHonorAsync(new CreateCompanyHonorDto { Name = "历史荣誉", AwardDate = new(2020, 1, 1), HonorLevel = "行业级" });
        await service.UpdateHonorAsync(honor.Id, new UpdateCompanyHonorDto { Version = (await db.Set<CompanyHonor>().FindAsync(honor.Id))!.Version, AwardDate = null, HonorLevel = null });
        Assert.Null((await service.GetHonorByIdAsync(honor.Id))!.AwardDate);
        Assert.Null((await service.GetHonorByIdAsync(honor.Id))!.HonorLevel);
    }

    [Fact]
    public async Task AttachmentReferencesProtectBodiesAndImagesAndBatchDeletionIsAllOrNothing()
    {
        await using var db = MemoryDb();
        db.Attachments.AddRange(new Attachment { Id = 1, FileName = "正文.jpg", FilePath = "uploads/body.jpg", FileUrl = "/uploads/body.jpg" },
            new Attachment { Id = 2, FileName = "空闲.txt", FilePath = "uploads/free.txt", FileUrl = "/uploads/free.txt" });
        db.InfoPublications.Add(new InfoPublication { Title = "扫描通知", Content = "<p><img src='/uploads/body.jpg'></p>" });
        await db.SaveChangesAsync();
        Assert.Single(await AttachmentReferences.FindAsync(db, 1));
        var service = Attachments(db);
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteRangeAsync([2, 1]));
        Assert.All(await db.Attachments.ToListAsync(), a => Assert.Equal(0, a.IsDeleted));
        Assert.True(await service.DeleteAsync(2));
        Assert.False(await service.DeleteAsync(999));
    }

    [Fact]
    public async Task DateFiltersIncludeTheEntireLastDayAndStatisticsAggregatePublicationDatesWithoutLoadingBodies()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var recorder = new QueryRecorder();
        await using var db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection).AddInterceptors(recorder).Options);
        await db.Database.ExecuteSqlRawAsync("""
            CREATE TABLE info_publications (id INTEGER PRIMARY KEY, type TEXT NOT NULL, category TEXT, title TEXT NOT NULL,
            document_number TEXT, summary TEXT, content TEXT NOT NULL, cover_image_id INTEGER, author TEXT, publisher TEXT,
            publish_time TEXT, view_count INTEGER NOT NULL, attachment_ids TEXT, is_top INTEGER NOT NULL, status INTEGER NOT NULL,
            is_deleted INTEGER NOT NULL, created_at TEXT NOT NULL, updated_at TEXT NOT NULL, version TEXT NOT NULL);
            """);
        var day = new DateTime(2024, 5, 6);
        db.InfoPublications.AddRange(
            new InfoPublication { Id = 1, Type = "COMPANY_NEWS", Title = "上午", PublishTime = day, Content = "正文", CreatedAt = DateTime.UtcNow },
            new InfoPublication { Id = 2, Type = "COMPANY_NEWS", Title = "深夜", PublishTime = day.AddHours(23).AddMinutes(59), Content = "正文", CreatedAt = DateTime.UtcNow },
            new InfoPublication { Id = 3, Type = "COMPANY_NEWS", Title = "次日", PublishTime = day.AddDays(1), Content = "正文" },
            new InfoPublication { Id = 4, Type = "COMPANY_NEWS", Title = "隐藏", PublishTime = day, Content = "正文", Status = 0 });
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        var repo = new InfoPublicationRepository(db);
        var admin = await repo.GetPagedPublicationsAsync("COMPANY_NEWS", null, null, 1, 10, day, day);
        Assert.Equal(3, admin.TotalCount);
        var portal = await repo.GetPagedPublicationsForPortalAsync("COMPANY_NEWS", null, null, 1, 1, day, day);
        Assert.Equal(2, portal.TotalCount);
        Assert.Equal(2, Assert.Single(portal.Items).Id);
        var future = await repo.GetPagedPublicationsAsync(null, null, null, 1, 10, day.AddYears(10), day.AddYears(10));
        Assert.Empty(future.Items);
        recorder.Commands.Clear();
        var stats = Publications(db);
        var trend = await stats.GetPublishTrendAsync(DateOnly.FromDateTime(day), DateOnly.FromDateTime(day), "day");
        Assert.Equal(2, Assert.Single(trend).Count);
        Assert.Equal("2024-05-06", trend[0].Date);
        Assert.Equal(4, (await stats.GetStatisticsOverviewAsync()).TotalPublications);
        Assert.All(recorder.Commands, sql => Assert.DoesNotContain("\"content\"", sql));
        Assert.Contains(recorder.Commands, sql => sql.Contains("GROUP BY"));
        Assert.Empty(db.ChangeTracker.Entries());
    }

    private sealed class QueryRecorder : DbCommandInterceptor
    {
        public List<string> Commands { get; } = [];
        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData,
            InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            Commands.Add(command.CommandText);
            return ValueTask.FromResult(result);
        }
    }
}
