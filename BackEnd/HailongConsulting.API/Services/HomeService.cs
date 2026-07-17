using HailongConsulting.API.Data;
using HailongConsulting.API.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Services;

/// <summary>
/// 首页服务实现
/// </summary>
public class HomeService : IHomeService
{
    private readonly ApplicationDbContext _context;

    public HomeService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取首页统计概览
    /// </summary>
    public async Task<HomeStatisticsDto> GetStatisticsOverviewAsync()
    {
        // 统计政府采购公告数量（按采购类型细分）
        var govGoodsCount = await _context.Announcements
            .Where(x => x.IsDeleted == 0 && x.BusinessType == "GOV_PROCUREMENT" && x.ProcurementType == "goods")
            .CountAsync();

        var govServiceCount = await _context.Announcements
            .Where(x => x.IsDeleted == 0 && x.BusinessType == "GOV_PROCUREMENT" && x.ProcurementType == "service")
            .CountAsync();

        var govProjectCount = await _context.Announcements
            .Where(x => x.IsDeleted == 0 && x.BusinessType == "GOV_PROCUREMENT" && x.ProcurementType == "project")
            .CountAsync();

        var govCount = govGoodsCount + govServiceCount + govProjectCount;

        // 统计建设工程公告数量
        var constructionCount = await _context.Announcements
            .Where(x => x.IsDeleted == 0 && x.BusinessType == "CONSTRUCTION")
            .CountAsync();

        // 总项目数
        var totalProjects = govCount + constructionCount;

        // 计算代理总额：统计 notice_type 为 result 的 budget_amount 金额总和
        var totalAmount = await _context.Announcements
            .Where(x => x.IsDeleted == 0 && x.NoticeType == "result" && x.BudgetAmount.HasValue)
            .SumAsync(x => x.BudgetAmount ?? 0);

        // 计算交易类型占比
        var projectTypes = new List<ProjectTypeStatDto>();
        
        if (totalProjects > 0)
        {
            // 政府采购（主类型，用于卡片显示）
            if (govCount > 0)
            {
                projectTypes.Add(new ProjectTypeStatDto
                {
                    Type = "政府采购",
                    Count = govCount,
                    Percentage = Math.Round((decimal)govCount * 100 / totalProjects, 2)
                });
            }

            // 政府采购 - 货物（子类型，用于饼图显示）
            if (govGoodsCount > 0)
            {
                projectTypes.Add(new ProjectTypeStatDto
                {
                    Type = "政府采购-货物",
                    Count = govGoodsCount,
                    Percentage = Math.Round((decimal)govGoodsCount * 100 / totalProjects, 2)
                });
            }

            // 政府采购 - 服务（子类型，用于饼图显示）
            if (govServiceCount > 0)
            {
                projectTypes.Add(new ProjectTypeStatDto
                {
                    Type = "政府采购-服务",
                    Count = govServiceCount,
                    Percentage = Math.Round((decimal)govServiceCount * 100 / totalProjects, 2)
                });
            }

            // 政府采购 - 工程（子类型，用于饼图显示）
            if (govProjectCount > 0)
            {
                projectTypes.Add(new ProjectTypeStatDto
                {
                    Type = "政府采购-工程",
                    Count = govProjectCount,
                    Percentage = Math.Round((decimal)govProjectCount * 100 / totalProjects, 2)
                });
            }

            // 建设工程
            if (constructionCount > 0)
            {
                projectTypes.Add(new ProjectTypeStatDto
                {
                    Type = "建设工程",
                    Count = constructionCount,
                    Percentage = Math.Round((decimal)constructionCount * 100 / totalProjects, 2)
                });
            }
        }

        // 统计地区排行（政府采购）- 包含项目数量和所有金额
        var govRegionStats = await _context.Announcements
            .Where(x => x.IsDeleted == 0 && x.BusinessType == "GOV_PROCUREMENT")
            .GroupBy(x => x.ProjectRegion)
            .Select(g => new
            {
                Region = g.Key,
                Count = g.Count(),
                Amount = g.Where(x => x.BudgetAmount.HasValue)
                         .Sum(x => x.BudgetAmount ?? 0)
            })
            .ToListAsync();

        // 统计地区排行（建设工程）- 包含项目数量和所有金额
        var constructionRegionStats = await _context.Announcements
            .Where(x => x.IsDeleted == 0 && x.BusinessType == "CONSTRUCTION")
            .GroupBy(x => x.ProjectRegion)
            .Select(g => new
            {
                Region = g.Key,
                Count = g.Count(),
                Amount = g.Where(x => x.BudgetAmount.HasValue)
                         .Sum(x => x.BudgetAmount ?? 0)
            })
            .ToListAsync();

        // 合并地区统计
        var regionRanking = govRegionStats
            .Concat(constructionRegionStats)
            .GroupBy(x => x.Region)
            .Select(g => new RegionRankingDto
            {
                Region = g.Key ?? "未设置地区",
                ProjectCount = g.Sum(x => x.Count),
                Amount = g.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.ProjectCount)
            .Take(5)
            .ToList();

        return new HomeStatisticsDto
        {
            TotalProjects = totalProjects,
            TotalAmount = totalAmount,
            ProjectTypes = projectTypes,
            RegionRanking = regionRanking
        };
    }

    /// <summary>
    /// 获取最新公告列表
    /// </summary>
    public async Task<List<RecentAnnouncementDto>> GetRecentAnnouncementsAsync()
    {
        // 获取最新5条政府采购公告
        var govAnnouncements = await _context.Announcements
            .Where(x => x.IsDeleted == 0 && x.BusinessType == "GOV_PROCUREMENT")
            .OrderByDescending(x => x.PublishTime)
            .Take(5)
            .ToListAsync();

        // 获取最新5条建设工程公告
        var constructionAnnouncements = await _context.Announcements
            .Where(x => x.IsDeleted == 0 && x.BusinessType == "CONSTRUCTION")
            .OrderByDescending(x => x.PublishTime)
            .Take(5)
            .ToListAsync();

        // 合并并转换为DTO
        var allAnnouncements = govAnnouncements
            .Select(x => new RecentAnnouncementDto
            {
                Id = (int)x.Id,
                Title = x.Title,
                NoticeType = x.NoticeType,
                NoticeTypeName = GetNoticeTypeName(x.NoticeType, x.BusinessType),
                ProjectRegion = x.ProjectRegion ?? string.Empty,
                PublishTime = x.PublishTime ?? x.CreatedAt,
                SourceType = "政府采购"
            })
            .Concat(constructionAnnouncements.Select(x => new RecentAnnouncementDto
            {
                Id = (int)x.Id,
                Title = x.Title,
                NoticeType = x.NoticeType,
                NoticeTypeName = GetNoticeTypeName(x.NoticeType, x.BusinessType),
                ProjectRegion = x.ProjectRegion ?? string.Empty,
                PublishTime = x.PublishTime ?? x.CreatedAt,
                SourceType = "建设工程"
            }))
            .OrderByDescending(x => x.PublishTime)
            .Take(10)
            .ToList();

        return allAnnouncements;
    }

    /// <summary>
    /// 获取重要业绩列表
    /// </summary>
    public async Task<List<AchievementDto>> GetAchievementsAsync()
    {
        var achievements = await _context.MajorAchievements
            .Where(x => x.IsDeleted == 0)
            .OrderBy(x => x.SortOrder)
            .ThenByDescending(x => x.CompletionDate)
            .Select(x => new AchievementDto
            {
                Id = x.Id,
                ProjectName = x.ProjectName,
                ProjectType = x.ProjectType,
                ProjectAmount = x.ProjectAmount,
                ClientName = x.ClientName,
                CompletionDate = x.CompletionDate,
                Description = x.Description,
                ImageUrl = null // 新实体使用ImageIds，这里暂时返回null
            })
            .ToListAsync();

        return achievements;
    }

    /// <summary>
    /// 获取公告类型中文名称
    /// </summary>
    private static string GetNoticeTypeName(string? noticeType, string? businessType)
    {
        if (string.IsNullOrEmpty(noticeType))
            return string.Empty;

        // 政府采购
        if (businessType == "GOV_PROCUREMENT")
        {
            return noticeType switch
            {
                "bidding" => "招标公告",
                "correction" => "更正公告",
                "result" => "结果公告",
                _ => noticeType
            };
        }
        // 建设工程
        else if (businessType == "CONSTRUCTION")
        {
            return noticeType switch
            {
                "bidding" => "采购公告",
                "correction" => "更正公告",
                "result" => "结果公告",
                _ => noticeType
            };
        }

        return noticeType;
    }
}
