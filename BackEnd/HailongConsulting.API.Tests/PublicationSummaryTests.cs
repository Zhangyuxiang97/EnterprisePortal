using HailongConsulting.API.Services;

namespace HailongConsulting.API.Tests;

public class PublicationSummaryTests
{
    [Fact]
    public void WordStylesNeverBecomeListText()
    {
        var html = "<style>@font-face {font-family:宋体}</style><script>bad()</script><p>采购&nbsp;通知</p><p>正文</p>";
        Assert.Equal("采购 通知 正文", PublicationSummary.Normalize(html, ""));
        Assert.Equal("采购 通知 正文", PublicationSummary.Normalize("@font-face{ font-family:宋体; }", html));
        Assert.Equal("采购 通知 正文", PublicationSummary.Normalize(null, html));
    }

    [Fact]
    public void ExplicitSummaryWinsAndScannedContentDoesNotExposeMarkup()
    {
        Assert.Equal("人工摘要", PublicationSummary.Normalize("<b>人工摘要</b>", "<p>正文</p>"));
        Assert.Equal("", PublicationSummary.Normalize("<p>&nbsp;</p>", "<p><img src='/uploads/legacy/a.jpg'></p>"));
        Assert.Equal(499, PublicationSummary.Normalize(new string('字', 499) + "😀", "").Length);
    }
}
