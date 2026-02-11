using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Varta.Store.Shared;

[Table("ServerTag")]
public class ServerTag
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(40)]
    public string Name { get; set; }

    [MaxLength(50)]
    public string? IpAddress { get; set; }

    [MaxLength(255)]
    public string? ImageUrl { get; set; }
}