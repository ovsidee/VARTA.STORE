namespace Varta.Store.Shared.DTO;

public class UserProfileDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string SteamID { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public List<OrderDto> Orders { get; set; } = new();
    public List<WalletTransactionDto> Transactions { get; set; } = new();
}
