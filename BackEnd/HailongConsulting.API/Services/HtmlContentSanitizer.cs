using Ganss.Xss;
using System.Text.RegularExpressions;

namespace HailongConsulting.API.Services;

public interface IHtmlContentSanitizer
{
    string Sanitize(string? html);
}

/// <summary>
/// Applies the same server-side allowlist to every HTML field before it is stored or returned.
/// The portal uses v-html, so editor-side filtering alone is not a security boundary.
/// </summary>
public sealed class HtmlContentSanitizer : IHtmlContentSanitizer
{
    private readonly HtmlSanitizer _sanitizer = new();
    private static readonly Regex InlineRasterImage = new(
        @"\Adata:image/(?:png|jpeg|gif|webp);base64,[a-z0-9+/=\s]+\z",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    public HtmlContentSanitizer()
    {
        _sanitizer.AllowedSchemes.Clear();
        _sanitizer.AllowedSchemes.Add("http");
        _sanitizer.AllowedSchemes.Add("https");
        _sanitizer.AllowedSchemes.Add("mailto");
        // 恢复的内嵌图片只允许出现在 img.src，不全局放开 data URI。
        _sanitizer.RemovingAttribute += (_, e) =>
        {
            if (e.Reason == RemoveReason.NotAllowedUrlValue &&
                e.Tag.LocalName == "img" && e.Attribute.Name == "src" &&
                InlineRasterImage.IsMatch(e.Attribute.Value))
                e.Cancel = true;
        };
    }

    public string Sanitize(string? html) =>
        string.IsNullOrWhiteSpace(html) ? string.Empty : _sanitizer.Sanitize(html);
}
