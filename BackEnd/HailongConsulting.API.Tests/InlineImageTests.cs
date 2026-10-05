using HailongConsulting.API.Services;

namespace HailongConsulting.API.Tests;

public class InlineImageTests
{
    private const string Png = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+aCVcAAAAASUVORK5CYII=";

    [Fact]
    public void RecoveredInlinePngSurvivesApiSanitizingWhileEventsAreRemoved()
    {
        var html = new HtmlContentSanitizer().Sanitize($"<p>正文</p><img src=\"{Png}\" onerror=\"alert(1)\"><script>alert(2)</script>");
        Assert.Contains(Png, html);
        Assert.DoesNotContain("onerror", html);
        Assert.DoesNotContain("<script", html);
    }

    [Theory]
    [InlineData("<a href='data:image/png;base64,AAAA'>链接</a>")]
    [InlineData("<img src='data:image/svg+xml;base64,AAAA'>")]
    [InlineData("<img src='data:text/html;base64,AAAA'>")]
    [InlineData("<img src='javascript:alert(1)'>")]
    public void ActiveOrNonImageDataUrlsStayBlocked(string input)
    {
        var html = new HtmlContentSanitizer().Sanitize(input);
        Assert.DoesNotContain("data:", html);
        Assert.DoesNotContain("javascript:", html);
    }
}
