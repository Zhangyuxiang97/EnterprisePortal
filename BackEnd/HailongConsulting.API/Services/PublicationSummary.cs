using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using System.Text.RegularExpressions;

namespace HailongConsulting.API.Services;

/// <summary>列表摘要只返回可读文本；兼容旧导入中已经变成纯文本的 Word CSS。</summary>
public static class PublicationSummary
{
    public static string Normalize(string? summary, string? content)
    {
        var text = PlainText(summary);
        if (string.IsNullOrWhiteSpace(text) || Regex.IsMatch(text,
                @"@font-face\s*\{|\bmso-[\w-]+\s*:|[p.]?MsoNormal\s*\{", RegexOptions.IgnoreCase))
            text = PlainText(content);
        if (text.Length <= 500) return text;
        var length = char.IsHighSurrogate(text[499]) ? 499 : 500;
        return text[..length];
    }

    private static string PlainText(string? html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;
        var document = new HtmlParser().ParseDocument(html);
        foreach (var node in document.QuerySelectorAll("script,style,noscript,template,iframe,object"))
            node.Remove();
        foreach (var node in document.QuerySelectorAll("p,div,br,li,h1,h2,h3,h4,h5,h6,tr,td,th,blockquote"))
            node.AppendChild(document.CreateTextNode(" "));
        return Regex.Replace(document.DocumentElement.TextContent, @"\s+", " ").Trim();
    }
}
