using AutoMapper;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Repositories;

namespace HailongConsulting.API.Services;

/// <summary>
/// 系统配置服务实现
/// </summary>
public class ConfigService : IConfigService
{
    private readonly IConfigRepository _repository;
    private readonly IMapper _mapper;
    private readonly IAttachmentService _attachmentService;
    private readonly IHtmlContentSanitizer _htmlContentSanitizer;

    public ConfigService(
        IConfigRepository repository,
        IMapper mapper,
        IAttachmentService attachmentService,
        IHtmlContentSanitizer htmlContentSanitizer)
    {
        _repository = repository;
        _mapper = mapper;
        _attachmentService = attachmentService;
        _htmlContentSanitizer = htmlContentSanitizer;
    }

    #region 轮播图

    public async Task<IEnumerable<CarouselBannerDto>> GetAllBannersAsync()
    {
        var banners = await _repository.GetAllBannersAsync();
        return _mapper.Map<IEnumerable<CarouselBannerDto>>(banners);
    }

    public async Task<CarouselBannerDto?> GetBannerByIdAsync(int id)
    {
        var banner = await _repository.GetBannerByIdAsync(id);
        return banner == null ? null : _mapper.Map<CarouselBannerDto>(banner);
    }

    public async Task<CarouselBannerDto> CreateBannerAsync(CreateCarouselBannerDto dto)
    {
        var banner = _mapper.Map<CarouselBanner>(dto);
        var created = await _repository.AddBannerAsync(banner);
        return _mapper.Map<CarouselBannerDto>(created);
    }

    public async Task<bool> UpdateBannerAsync(int id, UpdateCarouselBannerDto dto)
    {
        var banner = await _repository.GetBannerByIdAsync(id);
        if (banner == null) return false;

        // 只更新非null的字段
        if (dto.Title != null) banner.Title = dto.Title;
        if (dto.Description != null) banner.Description = dto.Description;
        if (dto.ImageId.HasValue) banner.ImageId = dto.ImageId.Value;
        if (dto.LinkUrl != null) banner.LinkUrl = dto.LinkUrl;
        if (dto.SortOrder.HasValue) banner.SortOrder = dto.SortOrder.Value;
        if (dto.Status.HasValue) banner.Status = (sbyte)(dto.Status.Value ? 1 : 0);

        return await _repository.UpdateBannerAsync(banner);
    }

    public async Task<bool> DeleteBannerAsync(int id)
    {
        return await _repository.DeleteBannerAsync(id);
    }

    #endregion

    #region 企业简介

    public async Task<CompanyProfileDto?> GetCompanyProfileAsync()
    {
        var profile = await _repository.GetCompanyProfileAsync();
        if (profile == null) return null;
        
        var dto = _mapper.Map<CompanyProfileDto>(profile);
        dto.Content = _htmlContentSanitizer.Sanitize(dto.Content);
        
        // 填充图片URL
        if (dto.ImageIds != null && dto.ImageIds.Any())
        {
            var attachments = await _attachmentService.GetByIdsAsync(dto.ImageIds);
            dto.ImageUrls = attachments.Select(a => a.FileUrl).ToList();
        }
        
        return dto;
    }

    public async Task<bool> UpdateCompanyProfileAsync(UpdateCompanyProfileDto dto)
    {
        var profile = _mapper.Map<CompanyProfile>(dto);
        profile.Content = _htmlContentSanitizer.Sanitize(profile.Content);
        return await _repository.UpdateCompanyProfileAsync(profile);
    }

    #endregion

    #region 重要业绩

    public async Task<IEnumerable<MajorAchievementDto>> GetAllAchievementsAsync()
    {
        var achievements = await _repository.GetAllAchievementsAsync();
        var dtos = _mapper.Map<IEnumerable<MajorAchievementDto>>(achievements).ToList();
        
        // 填充图片URL
        foreach (var dto in dtos)
        {
            if (dto.ImageIds != null && dto.ImageIds.Any())
            {
                var attachments = await _attachmentService.GetByIdsAsync(dto.ImageIds);
                dto.ImageUrls = attachments.Select(a => a.FileUrl).ToList();
            }
        }
        
        return dtos;
    }

    public async Task<MajorAchievementDto?> GetAchievementByIdAsync(int id)
    {
        var achievement = await _repository.GetAchievementByIdAsync(id);
        if (achievement == null) return null;
        
        var dto = _mapper.Map<MajorAchievementDto>(achievement);
        
        // 填充图片URL
        if (dto.ImageIds != null && dto.ImageIds.Any())
        {
            var attachments = await _attachmentService.GetByIdsAsync(dto.ImageIds);
            dto.ImageUrls = attachments.Select(a => a.FileUrl).ToList();
        }
        
        return dto;
    }

    public async Task<MajorAchievementDto> CreateAchievementAsync(CreateMajorAchievementDto dto)
    {
        var achievement = _mapper.Map<MajorAchievement>(dto);
        var created = await _repository.AddAchievementAsync(achievement);
        return _mapper.Map<MajorAchievementDto>(created);
    }

    public async Task<bool> UpdateAchievementAsync(int id, UpdateMajorAchievementDto dto)
    {
        var achievement = await _repository.GetAchievementByIdAsync(id);
        if (achievement == null) return false;

        // 只更新非null的字段
        if (dto.ProjectName != null) achievement.ProjectName = dto.ProjectName;
        if (dto.ProjectType != null) achievement.ProjectType = dto.ProjectType;
        if (dto.ProjectAmount.HasValue) achievement.ProjectAmount = dto.ProjectAmount;
        if (dto.ClientName != null) achievement.ClientName = dto.ClientName;
        if (dto.CompletionDate.HasValue) achievement.CompletionDate = dto.CompletionDate;
        if (dto.Description != null) achievement.Description = dto.Description;
        if (dto.ImageIds != null) achievement.ImageIds = System.Text.Json.JsonSerializer.Serialize(dto.ImageIds.Take(1).ToList());
        if (dto.SortOrder.HasValue) achievement.SortOrder = dto.SortOrder.Value;

        return await _repository.UpdateAchievementAsync(achievement);
    }

    public async Task<bool> DeleteAchievementAsync(int id)
    {
        return await _repository.DeleteAchievementAsync(id);
    }

    #endregion

    #region 企业荣誉

    public async Task<IEnumerable<CompanyHonorDto>> GetAllHonorsAsync()
    {
        var honors = await _repository.GetAllHonorsAsync();
        var dtos = _mapper.Map<IEnumerable<CompanyHonorDto>>(honors).ToList();

        // 填充图片URL
        foreach (var dto in dtos)
        {
            if (dto.ImageId.HasValue && dto.ImageId.Value > 0)
            {
                var attachment = await _attachmentService.GetByIdAsync(dto.ImageId.Value);
                if (attachment != null)
                {
                    dto.ImageUrl = attachment.FileUrl;
                }
            }
        }

        return dtos;
    }

    public async Task<CompanyHonorDto?> GetHonorByIdAsync(int id)
    {
        var honor = await _repository.GetHonorByIdAsync(id);
        if (honor == null) return null;

        var dto = _mapper.Map<CompanyHonorDto>(honor);

        // 填充图片URL
        if (dto.ImageId.HasValue && dto.ImageId.Value > 0)
        {
            var attachment = await _attachmentService.GetByIdAsync(dto.ImageId.Value);
            if (attachment != null)
            {
                dto.ImageUrl = attachment.FileUrl;
            }
        }

        return dto;
    }

    public async Task<CompanyHonorDto> CreateHonorAsync(CreateCompanyHonorDto dto)
    {
        var honor = _mapper.Map<CompanyHonor>(dto);
        var created = await _repository.AddHonorAsync(honor);
        return _mapper.Map<CompanyHonorDto>(created);
    }

    public async Task<bool> UpdateHonorAsync(int id, UpdateCompanyHonorDto dto)
    {
        var honor = await _repository.GetHonorByIdAsync(id);
        if (honor == null) return false;

        // 只更新非null的字段
        if (dto.Name != null) honor.Name = dto.Name;
        if (dto.Description != null) honor.Description = dto.Description;
        if (dto.ImageId.HasValue) honor.ImageId = dto.ImageId;
        if (dto.AwardOrganization != null) honor.AwardOrganization = dto.AwardOrganization;
        if (dto.AwardDate.HasValue) honor.AwardDate = dto.AwardDate;
        if (dto.CertificateNo != null) honor.CertificateNo = dto.CertificateNo;
        if (dto.HonorLevel != null) honor.HonorLevel = dto.HonorLevel;
        if (dto.SortOrder.HasValue) honor.SortOrder = dto.SortOrder.Value;
        if (dto.Status.HasValue) honor.Status = (sbyte)(dto.Status.Value ? 1 : 0);

        return await _repository.UpdateHonorAsync(honor);
    }

    public async Task<bool> DeleteHonorAsync(int id)
    {
        return await _repository.DeleteHonorAsync(id);
    }

    #endregion

    #region 友情链接

    public async Task<IEnumerable<FriendlyLinkDto>> GetAllLinksAsync()
    {
        var links = await _repository.GetAllLinksAsync();
        return _mapper.Map<IEnumerable<FriendlyLinkDto>>(links);
    }

    public async Task<FriendlyLinkDto?> GetLinkByIdAsync(int id)
    {
        var link = await _repository.GetLinkByIdAsync(id);
        return link == null ? null : _mapper.Map<FriendlyLinkDto>(link);
    }

    public async Task<FriendlyLinkDto> CreateLinkAsync(CreateFriendlyLinkDto dto)
    {
        var link = _mapper.Map<FriendlyLink>(dto);
        var created = await _repository.AddLinkAsync(link);
        return _mapper.Map<FriendlyLinkDto>(created);
    }

    public async Task<bool> UpdateLinkAsync(int id, UpdateFriendlyLinkDto dto)
    {
        var link = await _repository.GetLinkByIdAsync(id);
        if (link == null) return false;

        // 只更新非null的字段
        if (dto.Name != null) link.Name = dto.Name;
        if (dto.Url != null) link.Url = dto.Url;
        if (dto.LogoId.HasValue) link.LogoId = dto.LogoId;
        if (dto.Description != null) link.Description = dto.Description;
        if (dto.SortOrder.HasValue) link.SortOrder = dto.SortOrder.Value;
        if (dto.Status.HasValue) link.Status = (sbyte)(dto.Status.Value ? 1 : 0);

        return await _repository.UpdateLinkAsync(link);
    }

    public async Task<bool> DeleteLinkAsync(int id)
    {
        return await _repository.DeleteLinkAsync(id);
    }

    #endregion

    #region 访问统计

    public async Task<VisitStatisticDto> GetVisitStatisticsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var yesterday = today.AddDays(-1);
        var firstDayOfMonth = new DateOnly(today.Year, today.Month, 1);

        var totalVisits = await _repository.GetTotalVisitsAsync();
        var todayVisits = await _repository.GetVisitsByDateAsync(today);
        var yesterdayVisits = await _repository.GetVisitsByDateAsync(yesterday);
        var thisMonthVisits = await _repository.GetVisitsByDateRangeAsync(firstDayOfMonth, today);

        return new VisitStatisticDto
        {
            TotalVisits = totalVisits,
            TodayVisits = todayVisits,
            YesterdayVisits = yesterdayVisits,
            ThisMonthVisits = thisMonthVisits
        };
    }

    public async Task RecordVisitAsync(RecordVisitDto dto, string? ipAddress, string? userAgent)
    {
        var statistic = new VisitStatistic
        {
            VisitDate = DateOnly.FromDateTime(DateTime.Today),
            PageUrl = dto.PageUrl,
            PageTitle = dto.PageTitle,
            VisitorIp = ipAddress,
            UserAgent = userAgent,
            Referer = dto.Referer,
            VisitCount = 1
        };

        await _repository.RecordVisitAsync(statistic);
    }
    #region 业务范围

    public async Task<IEnumerable<BusinessScopeDto>> GetAllBusinessScopesAsync()
    {
        var scopes = await _repository.GetAllBusinessScopesAsync();
        var dtos = _mapper.Map<IEnumerable<BusinessScopeDto>>(scopes).ToList();
        dtos.ForEach(dto => SanitizeDto(dto));
        
        // 填充图片URL
        foreach (var dto in dtos)
        {
            if (dto.ImageId.HasValue)
            {
                var attachment = await _attachmentService.GetByIdAsync(dto.ImageId.Value);
                if (attachment != null)
                {
                    dto.ImageUrl = attachment.FileUrl;
                }
            }
        }
        
        return dtos;
    }

    public async Task<BusinessScopeDto?> GetBusinessScopeByIdAsync(int id)
    {
        var scope = await _repository.GetBusinessScopeByIdAsync(id);
        if (scope == null) return null;
        
        var dto = _mapper.Map<BusinessScopeDto>(scope);
        dto = SanitizeDto(dto);
        
        // 填充图片URL
        if (dto.ImageId.HasValue)
        {
            var attachment = await _attachmentService.GetByIdAsync(dto.ImageId.Value);
            if (attachment != null)
            {
                dto.ImageUrl = attachment.FileUrl;
            }
        }
        
        return dto;
    }

    public async Task<BusinessScopeDto> CreateBusinessScopeAsync(CreateBusinessScopeDto dto)
    {
        var scope = _mapper.Map<BusinessScope>(dto);
        scope.Content = scope.Content == null ? null : _htmlContentSanitizer.Sanitize(scope.Content);
        var created = await _repository.AddBusinessScopeAsync(scope);
        return _mapper.Map<BusinessScopeDto>(created);
    }

    public async Task<bool> UpdateBusinessScopeAsync(int id, UpdateBusinessScopeDto dto)
    {
        var scope = await _repository.GetBusinessScopeByIdAsync(id);
        if (scope == null) return false;

        // 只更新非null的字段
        if (dto.Name != null) scope.Name = dto.Name;
        if (dto.Description != null) scope.Description = dto.Description;
        if (dto.Content != null) scope.Content = _htmlContentSanitizer.Sanitize(dto.Content);
        if (dto.Features != null) scope.Features = System.Text.Json.JsonSerializer.Serialize(dto.Features);
        if (dto.ImageId.HasValue) scope.ImageId = dto.ImageId;
        if (dto.SortOrder.HasValue) scope.SortOrder = dto.SortOrder.Value;
        if (dto.Status.HasValue) scope.Status = (sbyte)(dto.Status.Value ? 1 : 0);

        return await _repository.UpdateBusinessScopeAsync(scope);
    }

    private BusinessScopeDto SanitizeDto(BusinessScopeDto dto)
    {
        dto.Content = dto.Content == null ? null : _htmlContentSanitizer.Sanitize(dto.Content);
        return dto;
    }

    public async Task<bool> DeleteBusinessScopeAsync(int id)
    {
        return await _repository.DeleteBusinessScopeAsync(id);
    }

    #endregion

    #region 企业资质

    public async Task<IEnumerable<CompanyQualificationDto>> GetAllQualificationsAsync()
    {
        var qualifications = await _repository.GetAllQualificationsAsync();
        var dtos = _mapper.Map<IEnumerable<CompanyQualificationDto>>(qualifications).ToList();
        
        // 填充图片URL
        foreach (var dto in dtos)
        {
            if (dto.CertificateImageId.HasValue)
            {
                var attachment = await _attachmentService.GetByIdAsync(dto.CertificateImageId.Value);
                if (attachment != null)
                {
                    dto.CertificateImageUrl = attachment.FileUrl;
                }
            }
        }
        
        return dtos;
    }

    public async Task<CompanyQualificationDto?> GetQualificationByIdAsync(int id)
    {
        var qualification = await _repository.GetQualificationByIdAsync(id);
        if (qualification == null) return null;
        
        var dto = _mapper.Map<CompanyQualificationDto>(qualification);
        
        // 填充图片URL
        if (dto.CertificateImageId.HasValue)
        {
            var attachment = await _attachmentService.GetByIdAsync(dto.CertificateImageId.Value);
            if (attachment != null)
            {
                dto.CertificateImageUrl = attachment.FileUrl;
            }
        }
        
        return dto;
    }

    public async Task<CompanyQualificationDto> CreateQualificationAsync(CreateCompanyQualificationDto dto)
    {
        var qualification = _mapper.Map<CompanyQualification>(dto);
        var created = await _repository.AddQualificationAsync(qualification);
        return _mapper.Map<CompanyQualificationDto>(created);
    }

    public async Task<bool> UpdateQualificationAsync(int id, UpdateCompanyQualificationDto dto)
    {
        var qualification = await _repository.GetQualificationByIdAsync(id);
        if (qualification == null) return false;

        // 只更新非null的字段
        if (dto.Name != null) qualification.Name = dto.Name;
        if (dto.Description != null) qualification.Description = dto.Description;
        if (dto.CertificateImageId.HasValue) qualification.ImageId = dto.CertificateImageId;
        if (dto.CertificateNumber != null) qualification.CertificateNo = dto.CertificateNumber;
        if (dto.IssuingAuthority != null) qualification.IssuingAuthority = dto.IssuingAuthority;
        if (dto.IssueDate.HasValue) qualification.IssueDate = dto.IssueDate;
        if (dto.ExpiryDate.HasValue) qualification.ExpiryDate = dto.ExpiryDate;
        if (dto.SortOrder.HasValue) qualification.SortOrder = dto.SortOrder.Value;
        if (dto.Status.HasValue) qualification.Status = (sbyte)(dto.Status.Value ? 1 : 0);

        return await _repository.UpdateQualificationAsync(qualification);
    }

    public async Task<bool> DeleteQualificationAsync(int id)
    {
        return await _repository.DeleteQualificationAsync(id);
    }

    #endregion

    #endregion
}
