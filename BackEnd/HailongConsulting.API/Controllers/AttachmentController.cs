using HailongConsulting.API.Common;
using HailongConsulting.API.Common.Helpers;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Repositories;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 附件管理控制器
/// </summary>
[ApiController]
[Route("api/attachments")]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;
    private readonly IFileUploadService _fileUploadService;
    private readonly ILogger<AttachmentController> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly IUnitOfWork _unitOfWork;
    private readonly FileHelper _fileHelper;

    public AttachmentController(
        IAttachmentService attachmentService,
        IFileUploadService fileUploadService,
        ILogger<AttachmentController> logger,
        IWebHostEnvironment environment,
        IUnitOfWork unitOfWork,
        FileHelper fileHelper)
    {
        _attachmentService = attachmentService;
        _fileUploadService = fileUploadService;
        _logger = logger;
        _environment = environment;
        _unitOfWork = unitOfWork;
        _fileHelper = fileHelper;
    }

    /// <summary>
    /// 上传附件
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="category">分类（image/document/video/other）</param>
    /// <param name="relatedType">关联类型（如：announcement, info_publication等）</param>
    /// <param name="relatedId">关联ID</param>
    /// <returns>附件信息</returns>
    [HttpPost("upload")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<AttachmentDto>>> UploadAttachment(
        IFormFile file,
        [FromForm] string category = "other",
        [FromForm] string? relatedType = null,
        [FromForm] int? relatedId = null)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<AttachmentDto>.FailResult("文件不能为空"));
            }

            // 如果没有指定分类，根据文件扩展名自动识别
            if (string.IsNullOrEmpty(category) || category == "other")
            {
                category = _fileHelper.GetCategoryByExtension(file.FileName);
            }

            // 上传文件
            var uploadResult = await _fileUploadService.UploadFileAsync(file, category);
            if (!uploadResult.success)
            {
                return BadRequest(ApiResponse<AttachmentDto>.FailResult(uploadResult.errorMessage ?? "文件上传失败"));
            }


            // 创建附件实体
            var attachment = new Attachment
            {
                FileName = file.FileName ?? "unknown",
                FilePath = uploadResult.filePath ?? throw new InvalidOperationException("上传服务未返回文件路径"),
                FileUrl = $"{Request.Scheme}://{Request.Host}{uploadResult.filePath}",
                FileSize = file.Length,
                FileType = file.ContentType ?? "application/octet-stream",
                FileExtension = Path.GetExtension(file.FileName) ?? "",
                Category = category,
                RelatedType = relatedType,
                RelatedId = relatedId,
                IsDeleted = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _unitOfWork.Attachments.AddAsync(attachment);
            await _unitOfWork.SaveChangesAsync();

            var attachmentDto = new AttachmentDto
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                FilePath = attachment.FilePath,
                FileUrl = attachment.FileUrl,
                FileSize = attachment.FileSize,
                FileType = attachment.FileType,
                FileExtension = attachment.FileExtension,
                Category = attachment.Category,
                RelatedType = attachment.RelatedType,
                RelatedId = attachment.RelatedId,
                CreatedAt = attachment.CreatedAt
            };
            
            return Ok(ApiResponse<AttachmentDto>.SuccessResult(attachmentDto, "上传附件成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "上传附件失败");
            return StatusCode(500, ApiResponse<AttachmentDto>.FailResult($"上传附件失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 批量上传附件
    /// </summary>
    /// <param name="files">文件列表</param>
    /// <param name="category">分类</param>
    /// <param name="relatedType">关联类型</param>
    /// <param name="relatedId">关联ID</param>
    /// <returns>附件信息列表</returns>
    [HttpPost("upload/batch")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<List<AttachmentDto>>>> UploadAttachments(
        List<IFormFile> files,
        [FromForm] string category = "other",
        [FromForm] string? relatedType = null,
        [FromForm] int? relatedId = null)
    {
        try
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest(ApiResponse<List<AttachmentDto>>.FailResult("文件列表不能为空"));
            }

            var attachments = new List<AttachmentDto>();
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // 如果没有指定分类，根据文件扩展名自动识别
                    var fileCategory = category;
                    if (string.IsNullOrEmpty(fileCategory) || fileCategory == "other")
                    {
                        fileCategory = _fileHelper.GetCategoryByExtension(file.FileName);
                    }

                    // 上传文件
                    var uploadResult = await _fileUploadService.UploadFileAsync(file, fileCategory);
                    if (uploadResult.success)
                    {
                        // 验证必填字段
                        if (string.IsNullOrEmpty(uploadResult.filePath))
                        {
                            continue; // 跳过这个文件
                        }
            
                        // 创建附件实体
                        var attachment = new Attachment
                        {
                            FileName = file.FileName ?? "unknown",
                            FilePath = uploadResult.filePath,
                            FileUrl = $"{Request.Scheme}://{Request.Host}{uploadResult.filePath}",
                            FileSize = file.Length,
                            FileType = file.ContentType ?? "application/octet-stream",
                            FileExtension = Path.GetExtension(file.FileName) ?? "",
                            Category = fileCategory,
                            RelatedType = relatedType,
                            RelatedId = relatedId,
                            IsDeleted = 0,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        await _unitOfWork.Attachments.AddAsync(attachment);
                        await _unitOfWork.SaveChangesAsync();

                        var attachmentDto = new AttachmentDto
                        {
                            Id = attachment.Id,
                            FileName = attachment.FileName,
                            FilePath = attachment.FilePath,
                            FileUrl = attachment.FileUrl,
                            FileSize = attachment.FileSize,
                            FileType = attachment.FileType,
                            FileExtension = attachment.FileExtension,
                            Category = attachment.Category,
                            RelatedType = attachment.RelatedType,
                            RelatedId = attachment.RelatedId,
                            CreatedAt = attachment.CreatedAt
                        };

                        attachments.Add(attachmentDto);
                    }
                }
            }

            return Ok(ApiResponse<List<AttachmentDto>>.SuccessResult(attachments, $"成功上传 {attachments.Count} 个附件"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量上传附件失败");
            return StatusCode(500, ApiResponse<List<AttachmentDto>>.FailResult($"批量上传附件失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 获取附件列表（分页）
    /// </summary>
    /// <param name="page">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="category">分类筛选</param>
    /// <param name="relatedType">关联类型筛选</param>
    /// <param name="keyword">关键词搜索</param>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AttachmentDto>>>> GetAttachments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? category = null,
        [FromQuery] string? relatedType = null,
        [FromQuery] string? keyword = null)
    {
        try
        {
            var result = await _attachmentService.GetPagedAsync(page, pageSize, category, relatedType, keyword);
            return Ok(ApiResponse<PagedResult<AttachmentDto>>.SuccessResult(result, "获取附件列表成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取附件列表失败");
            return StatusCode(500, ApiResponse<PagedResult<AttachmentDto>>.FailResult("获取附件列表失败"));
        }
    }

    /// <summary>
    /// 根据ID获取附件
    /// </summary>
    /// <param name="id">附件ID</param>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<AttachmentDto>>> GetAttachment(int id)
    {
        try
        {
            var attachment = await _attachmentService.GetByIdAsync(id);
            if (attachment == null)
            {
                return NotFound(ApiResponse<AttachmentDto>.FailResult("附件不存在"));
            }
            return Ok(ApiResponse<AttachmentDto>.SuccessResult(attachment, "获取附件成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取附件失败，ID: {Id}", id);
            return StatusCode(500, ApiResponse<AttachmentDto>.FailResult("获取附件失败"));
        }
    }

    /// <summary>
    /// 根据关联类型和ID获取附件列表
    /// </summary>
    /// <param name="relatedType">关联类型</param>
    /// <param name="relatedId">关联ID</param>
    [HttpGet("by-relation")]
    public async Task<ActionResult<ApiResponse<List<AttachmentDto>>>> GetAttachmentsByRelation(
        [FromQuery] string relatedType,
        [FromQuery] int relatedId)
    {
        try
        {
            var attachments = await _attachmentService.GetByRelatedAsync(relatedType, relatedId);
            return Ok(ApiResponse<List<AttachmentDto>>.SuccessResult(attachments.ToList(), "获取附件列表成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取附件列表失败，RelatedType: {RelatedType}, RelatedId: {RelatedId}", relatedType, relatedId);
            return StatusCode(500, ApiResponse<List<AttachmentDto>>.FailResult("获取附件列表失败"));
        }
    }

    /// <summary>
    /// 根据ID列表批量获取附件
    /// </summary>
    /// <param name="ids">附件ID列表（逗号分隔）</param>
    [HttpGet("batch")]
    public async Task<ActionResult<ApiResponse<List<AttachmentDto>>>> GetAttachmentsByIds([FromQuery] string ids)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ids))
            {
                return BadRequest(ApiResponse<List<AttachmentDto>>.FailResult("ID列表不能为空"));
            }

            var idList = ids.Split(',')
                .Select(id => int.TryParse(id.Trim(), out var result) ? result : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

            if (idList.Count == 0)
            {
                return BadRequest(ApiResponse<List<AttachmentDto>>.FailResult("无效的ID列表"));
            }

            var attachments = await _attachmentService.GetByIdsAsync(idList);
            return Ok(ApiResponse<List<AttachmentDto>>.SuccessResult(attachments.ToList(), "获取附件列表成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量获取附件失败，IDs: {Ids}", ids);
            return StatusCode(500, ApiResponse<List<AttachmentDto>>.FailResult("批量获取附件失败"));
        }
    }

    /// <summary>
    /// 删除附件
    /// </summary>
    /// <param name="id">附件ID</param>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAttachment(int id)
    {
        try
        {
            var result = await _attachmentService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.FailResult("附件不存在"));
            }
            return Ok(ApiResponse<bool>.SuccessResult(true, "删除附件成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除附件失败，ID: {Id}", id);
            return StatusCode(500, ApiResponse<bool>.FailResult("删除附件失败"));
        }
    }

    /// <summary>
    /// 批量删除附件
    /// </summary>
    /// <param name="ids">附件ID列表</param>
    [HttpDelete("batch")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAttachments([FromBody] List<int> ids)
    {
        try
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest(ApiResponse<bool>.FailResult("ID列表不能为空"));
            }

            var result = await _attachmentService.DeleteRangeAsync(ids);
            return Ok(ApiResponse<bool>.SuccessResult(result, $"成功删除 {ids.Count} 个附件"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量删除附件失败");
            return StatusCode(500, ApiResponse<bool>.FailResult("批量删除附件失败"));
        }
    }

    /// <summary>
    /// 下载附件
    /// </summary>
    /// <param name="id">附件ID</param>
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadAttachment(int id)
    {
        try
        {
            var attachment = await _attachmentService.GetByIdAsync(id);
            if (attachment == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(_environment.WebRootPath, attachment.FilePath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, attachment.FileType ?? "application/octet-stream", attachment.FileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "下载附件失败，ID: {Id}", id);
            return StatusCode(500);
        }
    }
}
