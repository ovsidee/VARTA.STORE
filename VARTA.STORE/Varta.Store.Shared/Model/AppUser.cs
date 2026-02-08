using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Varta.Store.Shared;

[Table("AppUser")]
public class AppUser
{
    [Key]
    public int Id { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal Balance { get; set; }

    [Required]
    [MaxLength(30)]
    public string Username { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string SteamID { get; set; }
    
    [Required]
    [MaxLength(250)]
    public string AvatarUrl { get; set; }
    
    // nvg property for easier appuser.Orders...
    public List<Order> Orders { get; set; } = new();
}