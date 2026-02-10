using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Varta.Store.Shared;

[Table("WalletTransaction")]
public class WalletTransaction
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    // "Pending", "Completed", "Failed"
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    // ID from Donatik (payment_id) to prevent duplicates
    [MaxLength(100)]
    public string? ExternalTransactionId { get; set; }

    [ForeignKey(nameof(AppUser))]
    public int AppUserId { get; set; }

    public AppUser AppUser { get; set; }
}
