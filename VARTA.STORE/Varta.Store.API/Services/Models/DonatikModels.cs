using System.Text.Json.Serialization;

namespace Varta.Store.API.Services.Models;

public class DonatikResponse
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("data")]
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

    [JsonPropertyName("payment")]
    public DonatikPayment Payment { get; set; } = new();

    public decimal Amount => decimal.TryParse(Payment?.Amount, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var a) ? a : 0;

    public string Currency => Payment?.Currency ?? "UAH";

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

public class DonatikPayment
{
    [JsonPropertyName("amount")]
    public string Amount { get; set; } = "0";

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "UAH";
}
