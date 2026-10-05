using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using HailongConsulting.API.Models.Entities;

namespace HailongConsulting.API.Common;

public sealed class ContentConflictException : Exception
{
    public ContentConflictException() : base("内容已被其他人修改，请保留当前编辑，重新读取最新内容后再保存。") { }
}

public static class ContentRevision
{
    public static void Check(IVersionedContent entity, string? expected)
    {
        if (string.IsNullOrWhiteSpace(expected)) throw new ValidationException("请重新打开编辑页面后保存（缺少内容版本）。");
        if (!string.Equals(entity.Version, expected, StringComparison.Ordinal)) throw new ContentConflictException();
    }
}

public static class ApiErrors
{
    public static bool IsExpected(Exception ex) => ex is ValidationException or ContentConflictException or DbUpdateConcurrencyException;
}
