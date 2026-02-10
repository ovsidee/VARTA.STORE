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
        var token = _configuration["Donatik:Token"];
        if (string.IsNullOrEmpty(token) || token == "REPLACE_ME_LOCALLY")
        {
            _logger.LogError("Donatik Token is missing in configuration.");
            return new List<DonatikDonation>();
        }

        // URL based on user docs: http://api.donatik.io/donations?token=TOKEN&page=1&perPage=500
        // We might want to filter by date to optimize, but let's stick to last 500 for now.
        var url = $"http://api.donatik.io/donations?token={token}&page=1&perPage={limit}";

        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // The docs didn't specify the exact root wrapper, assuming standard "content" wrapper or array.
            // Based on typical pagination APIs, it's often an object with a 'content' or 'data' array.
            // Let's assume the mapped model structure is correct based on general observation, 
            // but we might need to adjust if the API returns a raw array.
            // The user provided the "Get sub user" type but not the specific "Get donations" response schema 
            // other than the params.
            // I'll try to deserialize to the DonatikResponse wrapper first.

            var result = JsonSerializer.Deserialize<DonatikResponse>(content, options);
            return result?.Content ?? new List<DonatikDonation>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch donations from Donatik.");
            return new List<DonatikDonation>();
        }
    }
}
