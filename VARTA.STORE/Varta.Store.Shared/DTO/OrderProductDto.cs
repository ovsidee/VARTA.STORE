namespace Varta.Store.Shared.DTO;

public class OrderProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal PriceAtPurchase { get; set; }
    public int Quantity { get; set; }
}
