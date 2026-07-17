using Ganss.Xss;

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

    public HtmlContentSanitizer()
    {
        _sanitizer.AllowedSchemes.Clear();
        _sanitizer.AllowedSchemes.Add("http");
        _sanitizer.AllowedSchemes.Add("https");
        _sanitizer.AllowedSchemes.Add("mailto");
    }

    public string Sanitize(string? html) =>
        string.IsNullOrWhiteSpace(html) ? string.Empty : _sanitizer.Sanitize(html);
}
