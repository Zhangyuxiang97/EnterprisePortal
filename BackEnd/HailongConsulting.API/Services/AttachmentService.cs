using AutoMapper;
using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Services;

/// <summary>
/// 附件服务实现
/// </summary>
public class AttachmentService : IAttachmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AttachmentService> _logger;

    public AttachmentService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AttachmentService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<AttachmentDto>> GetPagedAsync(int page, int pageSize, string? category = null, string? relatedType = null, string? keyword = null)
    {
        try
        {
            var (items, totalCount) = await _unitOfWork.Attachments.GetFilteredPagedAsync(
                page, pageSize, category, relatedType, keyword);
            var dtoItems = _mapper.Map<List<AttachmentDto>>(items);

            return new PagedResult<AttachmentDto>
            {
                Items = dtoItems,
                TotalCount = totalCount,
                PageIndex = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取分页附件列表失败");
            throw;
        }
    }

    public async Task<AttachmentDto> UploadAsync(UploadAttachmentDto uploadDto)
    {
        try
        {
            var attachment = _mapper.Map<Attachment>(uploadDto);
            attachment.CreatedAt = DateTime.UtcNow;
            attachment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Attachments.AddAsync(attachment);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<AttachmentDto>(attachment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "上传附件失败");
            throw;
        }
    }

    public async Task<AttachmentDto?> GetByIdAsync(int id)
    {
        var attachment = await _unitOfWork.Attachments.FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted == 0);
        return attachment == null ? null : _mapper.Map<AttachmentDto>(attachment);
    }

    public async Task<IEnumerable<AttachmentDto>> GetByRelatedAsync(string relatedType, int relatedId)
    {
        var attachments = await _unitOfWork.Attachments.GetByRelatedAsync(relatedType, relatedId);
        return _mapper.Map<IEnumerable<AttachmentDto>>(attachments);
    }

    public async Task<IEnumerable<AttachmentDto>> GetByIdsAsync(IEnumerable<int> ids)
    {
        var attachments = await _unitOfWork.Attachments.GetByIdsAsync(ids);
        return _mapper.Map<IEnumerable<AttachmentDto>>(attachments);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            await _unitOfWork.Attachments.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除附件失败，ID: {Id}", id);
            return false;
        }
    }

    public async Task<bool> DeleteRangeAsync(IEnumerable<int> ids)
    {
        try
        {
            await _unitOfWork.Attachments.SoftDeleteRangeAsync(ids);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量删除附件失败");
            return false;
        }
    }

    #region IAttachmentStatisticsExtension 实现

    public async Task<int> GetTotalCountAsync()
    {
        var attachments = await _unitOfWork.Attachments.FindAsync(a => a.IsDeleted == 0);
        return attachments.Count();
    }

    #endregion
}
