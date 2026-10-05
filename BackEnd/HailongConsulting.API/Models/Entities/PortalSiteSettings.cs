using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HailongConsulting.API.Models.Entities;

[Table("portal_site_settings")]
public class PortalSiteSettings
{
    [Key]
    [Column("id")]
    public byte Id { get; set; } = 1;

    [Required]
    [Column("content_json", TypeName = "longtext")]
    public string ContentJson { get; set; } = string.Empty;

    [ConcurrencyCheck]
    [MaxLength(32)]
    [Column("version")]
    public string Version { get; set; } = Guid.NewGuid().ToString("N");

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
