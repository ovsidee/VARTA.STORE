using System.ComponentModel.DataAnnotations;

namespace Varta.Store.Shared.DTO;

public class CreateOrderDto
{
    [Required]
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; }
}
