namespace HailongConsulting.API.Services;

internal static class StatisticsPeriod
{
    // CreatedAt 由写接口以 UTC 保存；录入的 PublishTime 为中国本地墙上时间。
    public static DateTime TodayUtc => DateTime.UtcNow.AddHours(8).Date.AddHours(-8);
    public static string Bucket(DateTime date, string groupBy) => groupBy.ToLowerInvariant() switch {
        "month" => date.ToString("yyyy-MM"),
        "week" => date.AddDays(-(int)date.DayOfWeek).ToString("yyyy-MM-dd"),
        _ => date.ToString("yyyy-MM-dd")
    };
}
