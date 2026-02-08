using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Varta.Store.Shared;

[Table("OrderProductHistory")]
[PrimaryKey(nameof(OrderId), nameof(ProductId))]
public class OrderProductHistory
{
    [ForeignKey(nameof(Order))]
    public int OrderId { get; set; }
    
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal PriceAtPurchase { get; set; }

    public int Quantity { get; set; } = 1;
    
    public Order Order { get; set; }
    public Product Product { get; set; }
}