using Microsoft.Extensions.Configuration;

namespace HailongConsulting.API.Common.Helpers
{
    public class FileHelper
    {
        private static readonly HashSet<string> AllowedCategories = new(StringComparer.OrdinalIgnoreCase)
        {
            "image", "document", "video", "other"
        };
        private readonly IConfiguration _configuration;
        private readonly string[] _allowedExtensions;
        private readonly long _maxFileSize;

        public FileHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            
            // 从配置读取允许的扩展名
            var imageExts = _configuration["FileUpload:AllowedImageExtensions"]?.Split(',') ?? Array.Empty<string>();
            var docExts = _configuration["FileUpload:AllowedDocumentExtensions"]?.Split(',') ?? Array.Empty<string>();
            var videoExts = _configuration["FileUpload:AllowedVideoExtensions"]?.Split(',') ?? Array.Empty<string>();
            _allowedExtensions = imageExts.Concat(docExts).Concat(videoExts).ToArray();
            
            // 从配置读取最大文件大小
            _maxFileSize = long.TryParse(_configuration["FileUpload:MaxFileSize"], out var size)
                ? size
                : 10 * 1024 * 1024; // 默认10MB
        }

        public bool IsValidFile(IFormFile? file, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (file == null || file.Length == 0)
            {
                errorMessage = "文件不能为空";
                return false;
            }

            if (file.Length > _maxFileSize)
            {
                errorMessage = $"文件大小不能超过 {_maxFileSize / 1024 / 1024}MB";
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                errorMessage = $"不支持的文件类型。允许的类型: {string.Join(", ", _allowedExtensions)}";
                return false;
            }

            if (!HasExpectedFileSignature(file, extension))
            {
                errorMessage = "文件内容与扩展名不匹配";
                return false;
            }

            return true;
        }

        public string GenerateUniqueFileName(string originalFileName)
        {
            var safeOriginalFileName = Path.GetFileName(originalFileName);
            var extension = Path.GetExtension(safeOriginalFileName).ToLowerInvariant();
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(safeOriginalFileName);
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var randomString = Guid.NewGuid().ToString("N").Substring(0, 8);
            return $"{fileNameWithoutExtension}_{timestamp}_{randomString}{extension}";
        }

        public string GetRelativePath(string category, string fileName)
        {
            category = NormalizeCategory(category);
            var uploadPath = _configuration["FileUpload:UploadPath"] ?? "uploads/attachments";
            var yearMonth = DateTime.Now.ToString("yyyyMM");
            return Path.Combine(uploadPath, category, yearMonth, fileName).Replace("\\", "/");
        }

        public string GetPhysicalPath(string webRootPath, string relativePath)
        {
            if (string.IsNullOrEmpty(webRootPath))
            {
                throw new ArgumentNullException(nameof(webRootPath), "WebRootPath 不能为空");
            }

            if (string.IsNullOrEmpty(relativePath))
            {
                throw new ArgumentNullException(nameof(relativePath), "RelativePath 不能为空");
            }

            var rootPath = Path.GetFullPath(webRootPath);
            var physicalPath = Path.GetFullPath(Path.Combine(rootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString())));
            if (!physicalPath.StartsWith(rootPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("文件路径超出上传根目录");
            }

            return physicalPath;
        }

        public void EnsureDirectoryExists(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new ArgumentNullException(nameof(directoryPath), "DirectoryPath 不能为空");
            }

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public bool DeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return false;
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 根据文件扩展名自动识别分类
        /// </summary>
        public string GetCategoryByExtension(string? fileName)
        {
            var extension = Path.GetExtension(fileName ?? string.Empty).ToLowerInvariant();
            
            // 图片类型
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".ico" };
            if (imageExtensions.Contains(extension))
            {
                return "image";
            }
            
            // 文档类型
            var documentExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".csv", ".rtf" };
            if (documentExtensions.Contains(extension))
            {
                return "document";
            }
            
            // 视频类型
            var videoExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".mkv", ".webm" };
            if (videoExtensions.Contains(extension))
            {
                return "video";
            }
            
            // 其他类型
            return "other";
        }

        public string NormalizeCategory(string? category)
        {
            var normalized = category?.Trim().ToLowerInvariant() ?? "other";
            if (!AllowedCategories.Contains(normalized))
            {
                throw new InvalidOperationException("无效的文件分类");
            }

            return normalized;
        }

        private static bool HasExpectedFileSignature(IFormFile file, string extension)
        {
            if (extension is ".txt") return true;

            using var stream = file.OpenReadStream();
            Span<byte> header = stackalloc byte[16];
            var bytesRead = stream.Read(header);
            var bytes = header[..bytesRead];

            return extension switch
            {
                ".jpg" or ".jpeg" => bytes.Length >= 3 && bytes[..3].SequenceEqual(new byte[] { 0xFF, 0xD8, 0xFF }),
                ".png" => bytes.Length >= 8 && bytes[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
                ".gif" => bytes.Length >= 6 && (bytes[..6].SequenceEqual("GIF87a"u8) || bytes[..6].SequenceEqual("GIF89a"u8)),
                ".bmp" => bytes.Length >= 2 && bytes[..2].SequenceEqual("BM"u8),
                ".webp" => bytes.Length >= 12 && bytes[..4].SequenceEqual("RIFF"u8) && bytes.Slice(8, 4).SequenceEqual("WEBP"u8),
                ".ico" => bytes.Length >= 4 && bytes[..4].SequenceEqual(new byte[] { 0x00, 0x00, 0x01, 0x00 }),
                ".pdf" => bytes.Length >= 5 && bytes[..5].SequenceEqual("%PDF-"u8),
                ".doc" or ".xls" or ".ppt" => bytes.Length >= 8 && bytes[..8].SequenceEqual(new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }),
                ".docx" or ".xlsx" or ".pptx" => bytes.Length >= 4 && bytes[..4].SequenceEqual(new byte[] { 0x50, 0x4B, 0x03, 0x04 }),
                ".mp4" or ".mov" => bytes.Length >= 8 && bytes.Slice(4, 4).SequenceEqual("ftyp"u8),
                ".avi" => bytes.Length >= 12 && bytes[..4].SequenceEqual("RIFF"u8) && bytes.Slice(8, 4).SequenceEqual("AVI "u8),
                ".flv" => bytes.Length >= 3 && bytes[..3].SequenceEqual("FLV"u8),
                ".mkv" or ".webm" => bytes.Length >= 4 && bytes[..4].SequenceEqual(new byte[] { 0x1A, 0x45, 0xDF, 0xA3 }),
                _ => false
            };
        }
    }
}
