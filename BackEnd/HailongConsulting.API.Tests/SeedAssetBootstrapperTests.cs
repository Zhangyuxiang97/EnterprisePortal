using HailongConsulting.API.Services;

namespace HailongConsulting.API.Tests;

public class SeedAssetBootstrapperTests
{
    [Fact]
    public void EmptyUploadVolumeIsFilledAndExistingUploadsArePreserved()
    {
        var root = Path.Combine(Path.GetTempPath(), "hailong-seed-" + Guid.NewGuid().ToString("N"));
        try
        {
            var source = Path.Combine(root, "source");
            var webRoot = Path.Combine(root, "wwwroot");
            Directory.CreateDirectory(source);
            File.WriteAllText(Path.Combine(source, "a.jpg"), "seed");
            File.WriteAllText(Path.Combine(source, "form.docx"), "document");
            File.WriteAllText(Path.Combine(source, "ignored.html"), "not an asset");
            Assert.Equal(2, SeedAssetBootstrapper.CopyMissing(source, webRoot));
            var destination = Path.Combine(webRoot, "uploads", "legacy", "a.jpg");
            File.WriteAllText(destination, "user replacement");
            Assert.Equal(0, SeedAssetBootstrapper.CopyMissing(source, webRoot));
            Assert.Equal("user replacement", File.ReadAllText(destination));
            Assert.Equal(2, Directory.GetFiles(Path.Combine(webRoot, "uploads", "legacy")).Length);
        }
        finally { Directory.Delete(root, recursive: true); }
    }
}
