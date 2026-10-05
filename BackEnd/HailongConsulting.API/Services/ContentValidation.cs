using System.ComponentModel.DataAnnotations;
using AngleSharp.Html.Parser;
using HailongConsulting.API.Models.Entities;

namespace HailongConsulting.API.Services;

public static class ContentValidation
{
    public static bool HasContent(string? html)
    {
        var document = new HtmlParser().ParseDocument(html ?? "");
        return !string.IsNullOrWhiteSpace(document.Body?.TextContent.Replace("\u00a0", " ").Replace("\u200b", "")) ||
            document.QuerySelector("img[src]") != null;
    }

    public static void Validate(Announcement item)
    {
        ValidateTitleAndContent(item.Title, item.Content);
        if (item.BusinessType is not ("GOV_PROCUREMENT" or "CONSTRUCTION")) throw new ValidationException("公告业务类型不正确");
        if (item.NoticeType is not ("bidding" or "correction" or "result")) throw new ValidationException("公告类型不正确");
        if (item.BusinessType == "GOV_PROCUREMENT" && item.ProcurementType is not ("goods" or "service" or "project"))
            throw new ValidationException("请选择货物、服务或工程采购分类");
        if (item.BusinessType == "CONSTRUCTION") item.ProcurementType = null;
        if (item.NoticeType != "result" && item.AwardAmount != null) throw new ValidationException("仅结果公告可以填写中标金额");
        if (item.Status is not (0 or 1)) throw new ValidationException("公告状态不正确");
    }

    public static void Validate(InfoPublication item)
    {
        ValidateTitleAndContent(item.Title, item.Content);
        string[] allowed = item.Type switch {
            "COMPANY_NEWS" => ["公司新闻", "行业动态", "通知公告", "知识资讯"],
            "POLICY_REGULATION" => ["国家政策", "地方政策", "行业法规"],
            _ => throw new ValidationException("信息类型不正确")
        };
        if (!string.IsNullOrEmpty(item.Category) && !allowed.Contains(item.Category)) throw new ValidationException("信息分类与所属栏目不匹配");
        if (item.Status is not (0 or 1)) throw new ValidationException("信息状态不正确");
    }

    private static void ValidateTitleAndContent(string? title, string? html)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length > 255) throw new ValidationException("标题不能为空且不能超过255个字符");
        if (!HasContent(html)) throw new ValidationException("正文不能为空，可输入文字或上传扫描图片");
    }
}
