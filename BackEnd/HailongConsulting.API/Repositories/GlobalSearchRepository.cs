using HailongConsulting.API.Data;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Repositories;

/// <summary>
/// 全局搜索Repository实现
/// </summary>
public class GlobalSearchRepository : IGlobalSearchRepository
{
    private readonly ApplicationDbContext _context;

    public GlobalSearchRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 全局搜索
    /// </summary>
    public async Task<GlobalSearchResponseDto> SearchAsync(GlobalSearchRequestDto request)
    {
        var results = new List<GlobalSearchItemDto>();

        // 搜索政府采购公告
        if (request.Category == "all" || request.Category == "gov_procurement")
        {
            var govQuery = _context.Announcements
                .Where(x => x.IsDeleted == 0 && x.BusinessType == "GOV_PROCUREMENT");

            // 关键词搜索
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                govQuery = govQuery.Where(x =>
                    EF.Functions.Like(x.Title, $"%{request.Keyword}%") ||
                    EF.Functions.Like(x.Bidder ?? "", $"%{request.Keyword}%") ||
                    EF.Functions.Like(x.Winner ?? "", $"%{request.Keyword}%"));
            }

            // 公告类型过滤
            if (!string.IsNullOrWhiteSpace(request.AnnouncementType))
            {
                govQuery = govQuery.Where(x => x.NoticeType == request.AnnouncementType);
            }

            // 区域过滤
            if (request.Region != null && request.Region.Any())
            {
                govQuery = govQuery.Where(x => request.Region.Contains(x.ProjectRegion ?? string.Empty));
            }

            // 时间范围过滤
            if (request.StartDate.HasValue)
            {
                govQuery = govQuery.Where(x => x.PublishTime >= request.StartDate.Value);
            }
            if (request.EndDate.HasValue)
            {
                govQuery = govQuery.Where(x => x.PublishTime <= request.EndDate.Value);
            }

            var govResults = await govQuery
                .OrderByDescending(x => x.PublishTime)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.NoticeType,
                    x.Bidder,
                    x.ProjectRegion,
                    x.PublishTime,
                    x.ViewCount
                })
                .ToListAsync();

            results.AddRange(govResults.Select(x => new GlobalSearchItemDto
            {
                Id = (int)x.Id,
                Title = HighlightKeyword(x.Title, request.Keyword),
                Category = "gov_procurement",
                Type = x.NoticeType,
                Tenderer = x.Bidder,
                Region = x.ProjectRegion ?? string.Empty,
                PublishDate = x.PublishTime,
                ViewCount = (int)x.ViewCount
            }));
        }

        // 搜索建设工程公告
        if (request.Category == "all" || request.Category == "construction")
        {
            var constructionQuery = _context.Announcements
                .Where(x => x.IsDeleted == 0 && x.BusinessType == "CONSTRUCTION");

            // 关键词搜索
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                constructionQuery = constructionQuery.Where(x =>
                    EF.Functions.Like(x.Title, $"%{request.Keyword}%") ||
                    EF.Functions.Like(x.Bidder ?? "", $"%{request.Keyword}%") ||
                    EF.Functions.Like(x.Winner ?? "", $"%{request.Keyword}%"));
            }

            // 公告类型过滤
            if (!string.IsNullOrWhiteSpace(request.AnnouncementType))
            {
                constructionQuery = constructionQuery.Where(x => x.NoticeType == request.AnnouncementType);
            }

            // 区域过滤
            if (request.Region != null && request.Region.Any())
            {
                constructionQuery = constructionQuery.Where(x => request.Region.Contains(x.ProjectRegion ?? string.Empty));
            }

            // 时间范围过滤
            if (request.StartDate.HasValue)
            {
                constructionQuery = constructionQuery.Where(x => x.PublishTime >= request.StartDate.Value);
            }
            if (request.EndDate.HasValue)
            {
                constructionQuery = constructionQuery.Where(x => x.PublishTime <= request.EndDate.Value);
            }

            var constructionResults = await constructionQuery
                .OrderByDescending(x => x.PublishTime)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.NoticeType,
                    x.Bidder,
                    x.ProjectRegion,
                    x.PublishTime,
                    x.ViewCount
                })
                .ToListAsync();

            results.AddRange(constructionResults.Select(x => new GlobalSearchItemDto
            {
                Id = (int)x.Id,
                Title = HighlightKeyword(x.Title, request.Keyword),
                Category = "construction",
                Type = x.NoticeType,
                Tenderer = x.Bidder,
                Region = x.ProjectRegion ?? string.Empty,
                PublishDate = x.PublishTime,
                ViewCount = (int)x.ViewCount
            }));
        }

        // 按发布时间倒序排序
        results = results.OrderByDescending(x => x.PublishDate).ToList();

        // 分页
        var total = results.Count;
        var pagedResults = results
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new GlobalSearchResponseDto
        {
            Total = total,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            Items = pagedResults
        };
    }

    /// <summary>
    /// 高亮关键词
    /// </summary>
    private string HighlightKeyword(string text, string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword) || string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        // 使用正则表达式进行不区分大小写的替换
        return System.Text.RegularExpressions.Regex.Replace(
            text,
            System.Text.RegularExpressions.Regex.Escape(keyword),
            match => $"<em>{match.Value}</em>",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase
        );
    }
}
