namespace Varta.Store.Shared.DTO;

public class GameClaimRequestDto
{
    public string SteamId { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
