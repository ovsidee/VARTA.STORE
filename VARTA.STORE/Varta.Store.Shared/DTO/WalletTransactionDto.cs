namespace Varta.Store.Shared.DTO;

public class WalletTransactionDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ExternalTransactionId { get; set; } = string.Empty;
}
