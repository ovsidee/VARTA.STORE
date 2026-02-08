using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Varta.Store.Shared;

[Table("Order")]
public class Order
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    
    [ForeignKey(nameof(AppUser))]
    public int AppUserId { get; set; }
    [ForeignKey(nameof(OrderStatus))]
    public int StatusId { get; set; }
    
    public AppUser AppUser { get; set; }
    public OrderStatus OrderStatus { get; set; }
    
    // nvg property for easier Order.Items...
    public List<OrderProductHistory> Items { get; set; } = new();
}