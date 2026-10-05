using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using AutoMapper;
using HailongConsulting.API.Common;
using HailongConsulting.API.Data;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Repositories;
using HailongConsulting.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace HailongConsulting.API.Tests;

public class AnnouncementAmountTests
{
    private static ApplicationDbContext Database() => new(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static AnnouncementService Service(ApplicationDbContext context)
    {
        var mapper = new MapperConfiguration(c => c.AddProfile<MappingProfile>(), NullLoggerFactory.Instance).CreateMapper();
        return new AnnouncementService(new UnitOfWork(context), mapper,
            NullLogger<AnnouncementService>.Instance, new HtmlContentSanitizer());
    }

    private static CreateAnnouncementDto Result() => new()
    {
        Title = "金额分离测试", BusinessType = "GOV_PROCUREMENT", NoticeType = "result",
        ProcurementType = "service", BudgetAmount = 90m, AwardAmount = 86.9m, Content = "<p>结果正文</p>"
    };

    [Fact]
    public async Task CreateAndReadKeepBudgetAndAwardIndependent()
    {
        await using var db = Database();
        var service = Service(db);
        var created = await service.CreateAsync(Result());
        db.ChangeTracker.Clear();
        var read = await service.GetByIdAsync(created.Id);
        Assert.Equal(90m, read!.BudgetAmount);
        Assert.Equal(86.9m, read.AwardAmount);
    }

    [Fact]
    public async Task ExplicitJsonNullClearsAmountsWhileOmittedFieldsStayUnchanged()
    {
        await using var db = Database();
        var service = Service(db);
        var created = await service.CreateAsync(Result());
        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var omit = JsonSerializer.Deserialize<UpdateAnnouncementDto>("{\"title\":\"修改标题\"}", jsonOptions)!;
        omit.Version = created.Version;
        var kept = await service.UpdateAsync(created.Id, omit);
        Assert.Equal(90m, kept!.BudgetAmount);
        Assert.Equal(86.9m, kept.AwardAmount);
        var clear = JsonSerializer.Deserialize<UpdateAnnouncementDto>("{\"budgetAmount\":null,\"awardAmount\":null}", jsonOptions)!;
        clear.Version = (await db.Set<Announcement>().FindAsync(created.Id))!.Version;
        await service.UpdateAsync(created.Id, clear);
        db.ChangeTracker.Clear();
        var read = await service.GetByIdAsync(created.Id);
        Assert.Null(read!.BudgetAmount);
        Assert.Null(read.AwardAmount);
    }

    [Fact]
    public async Task UpdatingAwardDoesNotOverwriteBudgetAndAllowsZero()
    {
        await using var db = Database();
        var service = Service(db);
        var created = await service.CreateAsync(Result());
        var updated = await service.UpdateAsync(created.Id, new UpdateAnnouncementDto { Version = (await db.Set<Announcement>().FindAsync(created.Id))!.Version, AwardAmount = 0m });
        Assert.Equal(90m, updated!.BudgetAmount);
        Assert.Equal(0m, updated.AwardAmount);
    }

    [Fact]
    public async Task HomeTotalsUseResultAwardsAndCountUnclassifiedProcurement()
    {
        await using var db = Database();
        db.Announcements.AddRange(
            new Announcement { Title = "采购", BusinessType = "GOV_PROCUREMENT", NoticeType = "bidding", BudgetAmount = 100m, ProjectRegion = "河南省 郑州市" },
            new Announcement { Title = "成交", BusinessType = "GOV_PROCUREMENT", NoticeType = "result", BudgetAmount = 100m, AwardAmount = 80m, ProjectRegion = "河南省 郑州市" },
            new Announcement { Title = "工程结果", BusinessType = "CONSTRUCTION", NoticeType = "result", AwardAmount = 20m, ProjectRegion = "河南省 郑州市" },
            new Announcement { Title = "已删除结果", BusinessType = "GOV_PROCUREMENT", NoticeType = "result", AwardAmount = 999m, IsDeleted = 1 },
            new Announcement { Title = "非结果金额", BusinessType = "GOV_PROCUREMENT", NoticeType = "bidding", AwardAmount = 888m });
        await db.SaveChangesAsync();
        var stats = await new HomeService(db).GetStatisticsOverviewAsync();
        Assert.Equal(4, stats.TotalProjects);
        Assert.Equal(100m, stats.TotalAmount);
        Assert.Equal(100m, stats.RegionRanking.Single(x => x.Region == "河南省 郑州市").Amount);
        Assert.Equal(3, stats.ProjectTypes.Single(x => x.Type == "政府采购").Count);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(10000000000000)]
    public void InvalidAmountsFailDtoValidation(long value)
    {
        var dto = Result();
        dto.AwardAmount = value;
        Assert.False(Validator.TryValidateObject(dto, new ValidationContext(dto), new List<ValidationResult>(), true));
    }
}
