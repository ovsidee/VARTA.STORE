using System.Text.Json.Serialization;

namespace Varta.Store.API.Services.Models;

public class DonatikResponse
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("content")]
    public List<DonatikDonation> Content { get; set; } = new();
}

public class DonatikDonation
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "UAH";

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}
