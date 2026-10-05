using System.Linq.Expressions;
using HailongConsulting.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Services;

public record AttachmentReference(string Type, int Id, string Title);

/// <summary>一次检查整组选中文件；查询次数取决于内容表数量，不随附件数量增长。</summary>
public static class AttachmentReferences
{
    private sealed class Candidate
    {
        public string Type { get; init; } = "";
        public int Id { get; init; }
        public string Title { get; init; } = "";
        public string? Body { get; init; }
        public string? Ids { get; init; }
        public int? ImageId { get; init; }
    }

    public static async Task<List<AttachmentReference>> FindAsync(ApplicationDbContext db, int id)
        => (await FindManyAsync(db, [id])).GetValueOrDefault(id) ?? [];

    public static async Task<Dictionary<int, List<AttachmentReference>>> FindManyAsync(ApplicationDbContext db, IEnumerable<int> ids)
    {
        var selected = ids.Distinct().ToArray();
        var files = await db.Attachments.AsNoTracking().Where(a => selected.Contains(a.Id) && a.IsDeleted == 0)
            .Select(a => new { a.Id, a.FileUrl }).ToListAsync();
        var result = files.ToDictionary(a => a.Id, _ => new List<AttachmentReference>());
        if (files.Count == 0) return result;
        Expression<Func<Candidate, bool>> predicate = c => false;
        foreach (var file in files)
        {
            var id = file.Id;
            var token = $",{id},";
            var url = string.IsNullOrEmpty(file.FileUrl) ? "/uploads/__missing_attachment__" : file.FileUrl;
            Expression<Func<Candidate, bool>> condition = c => c.ImageId == id ||
                (c.Body != null && c.Body.Contains(url)) ||
                (c.Ids != null && c.Ids.Replace(" ", "").Replace("[", ",").Replace("]", ",").Contains(token));
            var right = new ReplaceParameter(condition.Parameters[0], predicate.Parameters[0]).Visit(condition.Body)!;
            predicate = Expression.Lambda<Func<Candidate, bool>>(Expression.OrElse(predicate.Body, right), predicate.Parameters);
        }

        async Task Collect(IQueryable<Candidate> query)
        {
            foreach (var row in await query.Where(predicate).ToListAsync())
            foreach (var file in files)
            {
                var token = $",{file.Id},";
                var referenced = row.ImageId == file.Id ||
                    (!string.IsNullOrEmpty(file.FileUrl) && row.Body?.Contains(file.FileUrl, StringComparison.Ordinal) == true) ||
                    row.Ids?.Replace(" ", "").Replace("[", ",").Replace("]", ",").Contains(token, StringComparison.Ordinal) == true;
                if (referenced) result[file.Id].Add(new(row.Type, row.Id, row.Title));
            }
        }

        await Collect(db.Announcements.AsNoTracking().Where(a => a.IsDeleted == 0)
            .Select(a => new Candidate { Type = "公告", Id = a.Id, Title = a.Title, Body = a.Content, Ids = a.AttachmentIds, ImageId = null }));
        await Collect(db.InfoPublications.AsNoTracking().Where(a => a.IsDeleted == 0)
            .Select(a => new Candidate { Type = "资讯", Id = a.Id, Title = a.Title, Body = a.Content, Ids = a.AttachmentIds, ImageId = a.CoverImageId }));
        await Collect(db.CompanyProfiles.AsNoTracking().Where(a => a.IsDeleted == 0)
            .Select(a => new Candidate { Type = "企业简介", Id = a.Id, Title = a.Title, Body = a.Content, Ids = a.ImageIds, ImageId = null }));
        await Collect(db.CompanyQualifications.AsNoTracking().Where(a => a.IsDeleted == 0)
            .Select(a => new Candidate { Type = "企业资质", Id = a.Id, Title = a.Name, Body = null, Ids = null, ImageId = a.ImageId }));
        await Collect(db.CompanyHonors.AsNoTracking().Where(a => a.IsDeleted == 0)
            .Select(a => new Candidate { Type = "企业荣誉", Id = a.Id, Title = a.Name, Body = null, Ids = null, ImageId = a.ImageId }));
        await Collect(db.BusinessScopes.AsNoTracking().Where(a => a.IsDeleted == 0)
            .Select(a => new Candidate { Type = "业务范围", Id = a.Id, Title = a.Name, Body = a.Content, Ids = null, ImageId = a.ImageId }));
        await Collect(db.MajorAchievements.AsNoTracking().Where(a => a.IsDeleted == 0)
            .Select(a => new Candidate { Type = "重要业绩", Id = a.Id, Title = a.ProjectName, Body = a.Description, Ids = a.ImageIds, ImageId = null }));
        await Collect(db.CarouselBanners.AsNoTracking().Where(a => a.IsDeleted == 0)
            .Select(a => new Candidate { Type = "轮播图", Id = a.Id, Title = a.Title, Body = null, Ids = null, ImageId = a.ImageId }));
        await Collect(db.FriendlyLinks.AsNoTracking().Where(a => a.IsDeleted == 0)
            .Select(a => new Candidate { Type = "友情链接", Id = a.Id, Title = a.Name, Body = null, Ids = null, ImageId = a.LogoId }));
        await Collect(db.PortalSiteSettings.AsNoTracking().Select(a => new Candidate { Type = "门户设置", Id = 1, Title = "门户站点设置", Body = a.ContentJson, Ids = null, ImageId = null }));
        return result;
    }

    private sealed class ReplaceParameter(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) => node == source ? target : base.VisitParameter(node);
    }
}
