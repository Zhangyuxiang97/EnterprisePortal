using HailongConsulting.API.Common.Helpers;

namespace HailongConsulting.API.Services
{
    public interface IFileUploadService
    {
        Task<(bool success, string? filePath, string? errorMessage)> UploadFileAsync(IFormFile file, string category);
        Task<List<string>> UploadMultipleFilesAsync(List<IFormFile> files, string category);
        bool DeleteFile(string webRootPath, string relativePath);
    }

    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileUploadService> _logger;
        private readonly FileHelper _fileHelper;

        public FileUploadService(
            IWebHostEnvironment environment,
            ILogger<FileUploadService> logger,
            FileHelper fileHelper)
        {
            _environment = environment;
            _logger = logger;
            _fileHelper = fileHelper;
        }

        public async Task<(bool success, string? filePath, string? errorMessage)> UploadFileAsync(IFormFile file, string category)
        {
            try
            {
                if (!_fileHelper.IsValidFile(file, out string errorMessage))
                {
                    return (false, null, errorMessage);
                }

                category = _fileHelper.NormalizeCategory(category);
                var uniqueFileName = _fileHelper.GenerateUniqueFileName(file.FileName);
                var relativePath = _fileHelper.GetRelativePath(category, uniqueFileName);
                var physicalPath = _fileHelper.GetPhysicalPath(_environment.WebRootPath, relativePath);

                var directory = Path.GetDirectoryName(physicalPath);
                if (directory != null)
                {
                    _fileHelper.EnsureDirectoryExists(directory);
                }

                await using (var stream = new FileStream(physicalPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation($"文件上传成功: {relativePath}");
                return (true, "/" + relativePath, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "文件上传失败");
                return (false, null, "文件上传失败: " + ex.Message);
            }
        }

        public async Task<List<string>> UploadMultipleFilesAsync(List<IFormFile> files, string category)
        {
            var uploadedFiles = new List<string>();

            foreach (var file in files)
            {
                var result = await UploadFileAsync(file, category);
                if (result.success && result.filePath != null)
                {
                    uploadedFiles.Add(result.filePath);
                }
            }

            return uploadedFiles;
        }

        public bool DeleteFile(string webRootPath, string relativePath)
        {
            var physicalPath = _fileHelper.GetPhysicalPath(webRootPath, relativePath.TrimStart('/'));
            return _fileHelper.DeleteFile(physicalPath);
        }
    }
}
