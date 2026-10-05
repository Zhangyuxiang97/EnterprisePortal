namespace HailongConsulting.API.Services;

/// <summary>将随版本发布的旧站原图补入上传卷，已有文件保持不变。</summary>
public static class SeedAssetBootstrapper
{
    public static int CopyMissing(string sourceDirectory, string webRootPath)
    {
        if (!Directory.Exists(sourceDirectory)) return 0;
        var targetDirectory = Path.Combine(webRootPath, "uploads", "legacy");
        Directory.CreateDirectory(targetDirectory);
        var copied = 0;
        foreach (var source in Directory.EnumerateFiles(sourceDirectory)
                     .Where(file => Path.GetExtension(file) is ".jpg" or ".docx"))
        {
            var target = Path.Combine(targetDirectory, Path.GetFileName(source));
            if (File.Exists(target)) continue;
            // 先写临时文件，再原子移动；中断不会留下半张图片供下次启动误判。
            var temporary = Path.Combine(targetDirectory, $".{Guid.NewGuid():N}.tmp");
            try
            {
                File.Copy(source, temporary, overwrite: false);
                try { File.Move(temporary, target, overwrite: false); copied++; }
                catch (IOException) when (File.Exists(target)) { }
            }
            finally
            {
                if (File.Exists(temporary)) File.Delete(temporary);
            }
        }
        return copied;
    }
}
