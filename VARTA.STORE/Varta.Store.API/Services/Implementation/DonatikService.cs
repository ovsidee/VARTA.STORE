using System.Text.Json;
using Varta.Store.API.Services.Interfaces;
using Varta.Store.API.Services.Models;

namespace Varta.Store.API.Services.Implementation;

public class DonatikService : IDonatikService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DonatikService> _logger;

    public DonatikService(HttpClient httpClient, IConfiguration configuration, ILogger<DonatikService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<List<DonatikDonation>> GetRecentDonationsAsync(int limit = 500, string? filterName = null)
    {
        // Try to get token from Env var first, then config
        var token = Environment.GetEnvironmentVariable("DONATIK_TOKEN")
                    ?? _configuration["Donatik:Token"];

        if (string.IsNullOrEmpty(token) || token == "REPLACE_ME_LOCALLY")
        {
            _logger.LogError("Donatik Token is missing in configuration.");
            throw new InvalidOperationException("Donatik API Token is missing or invalid in configuration. Please check 'DONATIK_TOKEN' environment variable or 'Donatik:Token' setting.");
        }

        // Default to last 30 days if not specified (though method sig doesn't have dates yet, we can add them later or just hardcode for "Recent")
        var toDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var fromDate = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-dd");

        // URL construction with query params:
        // http://api.donatik.io/donations?token=...&fromDate=...&toDate=...&page=1&perPage=500
        var builder = new UriBuilder("http://api.donatik.io/donations");
        var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
        query["token"] = token;
        query["fromDate"] = fromDate;
        query["toDate"] = toDate;
        query["page"] = "1";
        query["perPage"] = limit.ToString();

        builder.Query = query.ToString();
        var url = builder.ToString();

        try
        {
            // Remove header if it exists (clearing potential legacy usage)
            _httpClient.DefaultRequestHeaders.Remove("X-Token");

            _logger.LogInformation($"[DonatikService] Requesting: {url.Replace(token, "***")}");

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"[DonatikService] Failed. Status: {response.StatusCode}. Content: {content}");
                response.EnsureSuccessStatusCode();
            }

            // _logger.LogInformation($"[DonatikService] Response: {content}"); // Optional: log full response for debug

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<DonatikResponse>(content, options);

            if (result?.Content != null)
            {
                foreach (var d in result.Content)
                {
                    _logger.LogInformation($"[DonatikService] Parsed Donation: ID={d.Id}, Name={d.Name}, Amount={d.Amount} {d.Currency}");
                }
            }

            return result?.Content ?? new List<DonatikDonation>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch donations from Donatik.");
            return new List<DonatikDonation>();
        }
    }
}
