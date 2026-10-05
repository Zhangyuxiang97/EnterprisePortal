using HailongConsulting.API.Data;
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
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<AttachmentService> _logger;

    public AttachmentService(
        IUnitOfWork unitOfWork,
        ApplicationDbContext context,
        IMapper mapper,
        ILogger<AttachmentService> logger)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<AttachmentDto>> GetPagedAsync(int page, int pageSize, string? category = null, string? relatedType = null, string? keyword = null)
    {
        try
        {
            // 构建查询条件
            var query = _unitOfWork.Attachments.Query().Where(a => a.IsDeleted == 0);
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            // 分类筛选
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(a => a.Category == category);
            }

            // 关联类型筛选
            if (!string.IsNullOrWhiteSpace(relatedType))
            {
                query = query.Where(a => a.RelatedType == relatedType);
            }

            // 关键词搜索
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(a => a.FileName.Contains(keyword));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(a => a.CreatedAt).ThenByDescending(a => a.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

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
        if (!await _unitOfWork.Attachments.AnyAsync(a => a.Id == id && a.IsDeleted == 0)) return false;
        var references = await AttachmentReferences.FindAsync(_context, id);
        if (references.Count > 0) throw new InvalidOperationException($"附件正在被 {references.Count} 条内容引用，请先在对应内容中移除引用");
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
        var selected = ids.Distinct().ToArray();
        if (selected.Length is < 1 or > 100) throw new System.ComponentModel.DataAnnotations.ValidationException("每次请选择1到100个附件");
        if (await _unitOfWork.Attachments.Query().CountAsync(a => selected.Contains(a.Id) && a.IsDeleted == 0) != selected.Length) return false;
        var references = await AttachmentReferences.FindManyAsync(_context, selected);
        var used = references.FirstOrDefault(pair => pair.Value.Count > 0);
        if (used.Value != null) throw new InvalidOperationException($"附件 {used.Key} 仍被内容引用，本次未删除任何附件，请先移除引用");
        try
        {
            await _unitOfWork.Attachments.SoftDeleteRangeAsync(selected);
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
