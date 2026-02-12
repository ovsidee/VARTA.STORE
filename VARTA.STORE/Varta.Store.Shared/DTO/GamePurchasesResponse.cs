namespace Varta.Store.Shared.DTO;

public class GamePurchasesResponse
{
    public string SteamId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public List<GamePurchasedProductDto> Products { get; set; } = new();
}

public class GamePurchasedProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime PurchasedAt { get; set; }
}
