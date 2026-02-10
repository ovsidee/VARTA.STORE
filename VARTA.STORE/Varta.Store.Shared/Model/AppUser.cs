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

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(32)]
    public string Username { get; set; }

    [Required]
    [MaxLength(20)]
    public string SteamID { get; set; }

    [Required]
    [MaxLength(250)]
    public string AvatarUrl { get; set; }

    [Required]
    [MaxLength(250)]
    public string ProfileUrl { get; set; }

    [MaxLength(20)]
    public string Role { get; set; } = "User";

    // nvg property for easier appuser.Orders...
    public List<Order> Orders { get; set; } = new();
    public List<WalletTransaction> WalletTransactions { get; set; } = new();
}