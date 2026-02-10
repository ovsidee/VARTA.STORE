namespace Varta.Store.Shared.DTO;

public class OrderDto
{
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime DateCreated { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public List<OrderProductDto> Items { get; set; } = new();
}
