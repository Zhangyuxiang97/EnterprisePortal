using System.Text.Json.Serialization;

namespace HailongConsulting.API.Models.DTOs;

/// <summary>门户公开展示内容；导航和首页模块仍由前端静态配置管理。</summary>
public class PortalSiteSettingsDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string? Version { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public DateTime? UpdatedAt { get; set; }
    [JsonRequired] public PortalCompanyDto Company { get; set; } = new();
    [JsonRequired] public PortalContactDto Contact { get; set; } = new();
    [JsonRequired] public PortalTransportationDto Transportation { get; set; } = new();
    [JsonRequired] public List<PortalFaqDto> Faqs { get; set; } = [];
}

public class PortalCompanyDto
{
    public string FullName { get; set; } = string.Empty;
    public string Slogan { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class PortalContactDto
{
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    [JsonRequired] public PortalAddressDto Address { get; set; } = new();
    [JsonRequired] public PortalWorkingHoursDto WorkingHours { get; set; } = new();
    [JsonRequired] public PortalMapDto Map { get; set; } = new();
}

public class PortalAddressDto
{
    public string FullAddress { get; set; } = string.Empty;
}

public class PortalWorkingHoursDto
{
    public string Weekdays { get; set; } = string.Empty;
    public string Weekend { get; set; } = string.Empty;
}

public class PortalMapDto
{
    public string ApiKey { get; set; } = string.Empty;
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public int Zoom { get; set; } = 16;
}

public class PortalTransportationDto
{
    [JsonRequired] public PortalMetroDto Metro { get; set; } = new();
    [JsonRequired] public PortalBusDto Bus { get; set; } = new();
    [JsonRequired] public PortalDrivingDto Driving { get; set; } = new();
    [JsonRequired] public List<string> Landmarks { get; set; } = [];
}

public class PortalMetroDto
{
    public bool Enabled { get; set; }
    [JsonRequired] public List<PortalMetroLineDto> Lines { get; set; } = [];
}

public class PortalMetroLineDto
{
    public string Line { get; set; } = string.Empty;
    public string Station { get; set; } = string.Empty;
    public string Exit { get; set; } = string.Empty;
    public string WalkingDistance { get; set; } = string.Empty;
}

public class PortalBusDto
{
    public bool Enabled { get; set; }
    [JsonRequired] public List<string> Routes { get; set; } = [];
    public string Station { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class PortalDrivingDto
{
    public bool Enabled { get; set; }
    public string Navigation { get; set; } = string.Empty;
    public string Parking { get; set; } = string.Empty;
}

public class PortalFaqDto
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}
