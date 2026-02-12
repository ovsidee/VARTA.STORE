namespace Varta.Store.Shared.DTO;

public class GameProductOwnershipResponse
{
    public string SteamId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public bool Owned { get; set; }
    public int TotalQuantity { get; set; }
}
