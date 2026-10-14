using System.ComponentModel.DataAnnotations;

namespace HailongConsulting.API.Models.DTOs;

public class MoveCompanyContentDto
{
    [Required, RegularExpression("^(up|down)$")]
    public string Direction { get; set; } = string.Empty;

    [Required, StringLength(32, MinimumLength = 32)]
    public string Version { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int NeighborId { get; set; }

    [Required, StringLength(32, MinimumLength = 32)]
    public string NeighborVersion { get; set; } = string.Empty;
}
