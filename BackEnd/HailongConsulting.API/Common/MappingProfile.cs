using AutoMapper;
using HashidsNet;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;

namespace HailongConsulting.API.Common;

/// <summary>
/// AutoMapper映射配置
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User映射
        CreateMap<User, LoginResponseDto>();

        // 访问统计映射
        CreateMap<VisitStatistic, VisitStatisticDetailDto>()
            .ForMember(dest => dest.VisitDate, opt => opt.MapFrom(src => src.VisitDate.ToString("yyyy-MM-dd")));

        // 系统配置映射
        // 轮播图
        CreateMap<CarouselBanner, CarouselBannerDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == 1));
        
        CreateMap<CreateCarouselBannerDto, CarouselBanner>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (sbyte)(src.Status ? 1 : 0)));
        
        CreateMap<UpdateCarouselBannerDto, CarouselBanner>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.HasValue ? (sbyte)(src.Status.Value ? 1 : 0) : (sbyte?)null))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // 企业简介
        CreateMap<CompanyProfile, CompanyProfileDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == 1))
            .ForMember(dest => dest.Highlights, opt => opt.MapFrom(src => DeserializeStringList(src.Highlights)))
            .ForMember(dest => dest.ImageIds, opt => opt.MapFrom(src => DeserializeintList(src.ImageIds)));
        
        CreateMap<UpdateCompanyProfileDto, CompanyProfile>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.HasValue ? (sbyte)(src.Status.Value ? 1 : 0) : (sbyte?)null))
            .ForMember(dest => dest.Highlights, opt => opt.MapFrom(src => SerializeStringList(src.Highlights)))
            .ForMember(dest => dest.ImageIds, opt => opt.MapFrom(src => SerializeintList(src.ImageIds)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // 重要业绩
        CreateMap<MajorAchievement, MajorAchievementDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == 1))
            .ForMember(dest => dest.ImageIds, opt => opt.MapFrom(src => DeserializeintList(src.ImageIds)));
        
        CreateMap<CreateMajorAchievementDto, MajorAchievement>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (sbyte)(src.Status ? 1 : 0)))
            .ForMember(dest => dest.ImageIds, opt => opt.MapFrom(src => SerializeintList(src.ImageIds)));
        
        CreateMap<UpdateMajorAchievementDto, MajorAchievement>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.HasValue ? (sbyte)(src.Status.Value ? 1 : 0) : (sbyte?)null))
            .ForMember(dest => dest.ImageIds, opt => opt.MapFrom(src => SerializeintList(src.ImageIds)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // 企业荣誉
        CreateMap<CompanyHonor, CompanyHonorDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == 1));
        
        CreateMap<CreateCompanyHonorDto, CompanyHonor>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (sbyte)(src.Status ? 1 : 0)));
        
        CreateMap<UpdateCompanyHonorDto, CompanyHonor>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.HasValue ? (sbyte)(src.Status.Value ? 1 : 0) : (sbyte?)null))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // 友情链接
        CreateMap<FriendlyLink, FriendlyLinkDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == 1));
        
        CreateMap<CreateFriendlyLinkDto, FriendlyLink>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (sbyte)(src.Status ? 1 : 0)));
        
        CreateMap<UpdateFriendlyLinkDto, FriendlyLink>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.HasValue ? (sbyte)(src.Status.Value ? 1 : 0) : (sbyte?)null))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // 业务范围
        CreateMap<BusinessScope, BusinessScopeDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == 1))
            .ForMember(dest => dest.Features, opt => opt.MapFrom(src => DeserializeStringList(src.Features)));
        
        CreateMap<CreateBusinessScopeDto, BusinessScope>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (sbyte)(src.Status ? 1 : 0)))
            .ForMember(dest => dest.Features, opt => opt.MapFrom(src => SerializeStringList(src.Features)));
        
        CreateMap<UpdateBusinessScopeDto, BusinessScope>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.HasValue ? (sbyte)(src.Status.Value ? 1 : 0) : (sbyte?)null))
            .ForMember(dest => dest.Features, opt => opt.MapFrom(src => SerializeStringList(src.Features)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // 企业资质
        CreateMap<CompanyQualification, CompanyQualificationDto>()
            .ForMember(dest => dest.CertificateNumber, opt => opt.MapFrom(src => src.CertificateNo))
            .ForMember(dest => dest.CertificateImageId, opt => opt.MapFrom(src => src.ImageId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == 1));
        
        CreateMap<CreateCompanyQualificationDto, CompanyQualification>()
            .ForMember(dest => dest.CertificateNo, opt => opt.MapFrom(src => src.CertificateNumber))
            .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.CertificateImageId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (sbyte)(src.Status ? 1 : 0)));
        
        CreateMap<UpdateCompanyQualificationDto, CompanyQualification>()
            .ForMember(dest => dest.CertificateNo, opt => opt.MapFrom(src => src.CertificateNumber))
            .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.CertificateImageId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.HasValue ? (sbyte)(src.Status.Value ? 1 : 0) : (sbyte?)null))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // 附件映射
        CreateMap<Attachment, AttachmentDto>();
        CreateMap<UploadAttachmentDto, Attachment>();

        // 公告映射
        CreateMap<Announcement, AnnouncementDto>()
            .ForMember(dest => dest.HashId, opt => opt.MapFrom(src => EncodeHashId(src.Id)))
            .ForMember(dest => dest.IsTop, opt => opt.MapFrom(src => src.IsTop == 1))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.AttachmentIds, opt => opt.MapFrom(src => DeserializeintList(src.AttachmentIds)))
            .ForMember(dest => dest.NoticeTypeName, opt => opt.MapFrom(src => GetNoticeTypeName(src.NoticeType, src.BusinessType)))
            .ForMember(dest => dest.ProcurementTypeName, opt => opt.MapFrom(src => GetProcurementTypeName(src.ProcurementType)));
        
        CreateMap<CreateAnnouncementDto, Announcement>()
            .ForMember(dest => dest.IsTop, opt => opt.MapFrom(src => (sbyte)(src.IsTop ? 1 : 0)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (sbyte)src.Status))
            .ForMember(dest => dest.AttachmentIds, opt => opt.MapFrom(src => SerializeintList(src.AttachmentIds)));
        
        CreateMap<UpdateAnnouncementDto, Announcement>()
            .ForMember(dest => dest.IsTop, opt => opt.MapFrom(src => src.IsTop.HasValue ? (sbyte)(src.IsTop.Value ? 1 : 0) : (sbyte?)null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.HasValue ? (sbyte)src.Status.Value : (sbyte?)null))
            .ForMember(dest => dest.AttachmentIds, opt => opt.MapFrom(src => SerializeintList(src.AttachmentIds)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // 信息发布映射
        CreateMap<InfoPublication, InfoPublicationDto>()
            .ForMember(dest => dest.IsTop, opt => opt.MapFrom(src => src.IsTop == 1))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.AttachmentIds, opt => opt.MapFrom(src => DeserializeintList(src.AttachmentIds)));
        
        CreateMap<CreateInfoPublicationDto, InfoPublication>()
            .ForMember(dest => dest.IsTop, opt => opt.MapFrom(src => (sbyte)(src.IsTop ? 1 : 0)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (sbyte)src.Status))
            .ForMember(dest => dest.AttachmentIds, opt => opt.MapFrom(src => SerializeintList(src.AttachmentIds)));
        
        CreateMap<UpdateInfoPublicationDto, InfoPublication>()
            .ForMember(dest => dest.IsTop, opt => opt.MapFrom(src => src.IsTop.HasValue ? (sbyte)(src.IsTop.Value ? 1 : 0) : (sbyte?)null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.HasValue ? (sbyte)src.Status.Value : (sbyte?)null))
            .ForMember(dest => dest.AttachmentIds, opt => opt.MapFrom(src => SerializeintList(src.AttachmentIds)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // 区域字典映射
        CreateMap<RegionDictionary, RegionDictionaryDto>();
        CreateMap<CreateRegionDictionaryDto, RegionDictionary>();
        CreateMap<UpdateRegionDictionaryDto, RegionDictionary>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // 用户映射
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // 系统日志映射
        CreateMap<SystemLog, SystemLogDto>();
        CreateMap<CreateSystemLogDto, SystemLog>();
    }

    // HashId编码（静态实例，用于AutoMapper映射）
    private static readonly Hashids _hashids = new Hashids("HailongConsulting2024Portal", 8);

    private static string EncodeHashId(int id)
    {
        return _hashids.Encode(id);
    }

    // 辅助方法：序列化和反序列化 JSON
    private static string? SerializeintList(List<int>? list)
    {
        if (list == null || list.Count == 0)
            return null;
        // 修复：保存完整的列表，而不是只保存第一个元素
        return System.Text.Json.JsonSerializer.Serialize(list);
    }

    private static List<int>? DeserializeintList(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return null;
        return System.Text.Json.JsonSerializer.Deserialize<List<int>>(json);
    }

    private static string? SerializeStringList(List<string>? list)
    {
        if (list == null || list.Count == 0)
            return null;
        return System.Text.Json.JsonSerializer.Serialize(list);
    }

    private static List<string>? DeserializeStringList(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return null;
        return System.Text.Json.JsonSerializer.Deserialize<List<string>>(json);
    }

    // 公告类型映射（根据业务类型返回不同的中文名称）
    private static string GetNoticeTypeName(string? noticeType, string? businessType)
    {
        if (string.IsNullOrEmpty(noticeType))
            return string.Empty;

        // 政府采购
        if (businessType == "GOV_PROCUREMENT")
        {
            return noticeType switch
            {
                "bidding" => "招标公告",
                "correction" => "更正公告",
                "result" => "结果公告",
                _ => noticeType
            };
        }
        // 建设工程
        else if (businessType == "CONSTRUCTION")
        {
            return noticeType switch
            {
                "bidding" => "采购公告",
                "correction" => "更正公告",
                "result" => "结果公告",
                _ => noticeType
            };
        }

        return noticeType;
    }

    // 采购类型映射
    private static string? GetProcurementTypeName(string? procurementType)
    {
        if (string.IsNullOrEmpty(procurementType))
            return null;

        return procurementType switch
        {
            "goods" => "货物",
            "service" => "服务",
            "project" => "工程",
            _ => procurementType
        };
    }
}