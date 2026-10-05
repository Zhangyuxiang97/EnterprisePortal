using System.ComponentModel.DataAnnotations;

namespace HailongConsulting.API.Common;

public static class PaginationValidation
{
    public static IEnumerable<ValidationResult> Validate(int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100 || (long)(page - 1) * pageSize > int.MaxValue)
            yield return new ValidationResult("分页参数无效：页码至少为1，每页最多100条，翻页范围不能超出限制。");
        if (startDate.HasValue && endDate.HasValue && startDate.Value.Date > endDate.Value.Date)
            yield return new ValidationResult("开始日期不能晚于结束日期。");
        if (endDate?.Date == DateTime.MaxValue.Date)
            yield return new ValidationResult("结束日期超出支持范围。");
    }
}
