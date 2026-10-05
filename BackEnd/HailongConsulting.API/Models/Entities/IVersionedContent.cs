namespace HailongConsulting.API.Models.Entities;

/// <summary>内容版本与浏览次数、统计更新时间相互独立。</summary>
public interface IVersionedContent
{
    string Version { get; set; }
}
