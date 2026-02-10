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
            throw new InvalidOperationException("Donatik API Token is missing or invalid in configuration. Please check 'Donatik:Token' setting.");
        }

        // URL based on user docs: https://api.donatik.io/donations?page=1&perPage=500
        // Token must be in X-Token header
        var url = $"https://api.donatik.io/donations?page=1&perPage={limit}";

        try
        {
            _httpClient.DefaultRequestHeaders.Remove("X-Token");
            _httpClient.DefaultRequestHeaders.Add("X-Token", token);

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"[DonatikService] Status: {response.StatusCode}. Raw Response: {content}");

            response.EnsureSuccessStatusCode();

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
