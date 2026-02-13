using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Varta.Store.API.Services.Interfaces;
using Varta.Store.API.Services.Models;

namespace Varta.Store.API.Services.Implementation;

public class DonatikService : IDonatikService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DonatikService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public DonatikService(HttpClient httpClient, IConfiguration configuration, ILogger<DonatikService> logger, IServiceProvider serviceProvider)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task<List<DonatikDonation>> GetRecentDonationsAsync(int limit = 500, string? filterName = null)
    {
        var token = Environment.GetEnvironmentVariable("DONATIK_TOKEN")
                    ?? _configuration["Donatik:Token"];

        if (string.IsNullOrEmpty(token) || token == "REPLACE_ME_LOCALLY")
        {
            _logger.LogError("Donatik Token is missing in configuration.");
            throw new InvalidOperationException("Donatik API Token is missing or invalid in configuration. Please check 'DONATIK_TOKEN' environment variable or 'Donatik:Token' setting.");
        }

        var toDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var fromDate = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-dd");

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

    public async Task<(bool Success, string Message)> ProcessDonationAsync(DonatikDonation donation)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<Data.StoreDbContext>();

            try
            {
                if (string.IsNullOrEmpty(donation.Id))
                {
                    var msg = "[DonatikService] Invalid payload: Missing Donation ID.";
                    _logger.LogWarning(msg);
                    return (false, msg);
                }

                // Idempotency Check
                var exists = await context.WalletTransactions.AnyAsync(t => t.ExternalTransactionId == donation.Id);
                if (exists)
                {
                    var msg = $"[DonatikService] Skipped {donation.Id}. Reason: Already processed.";
                    _logger.LogInformation(msg);
                    return (false, msg);
                }

                // Find User
                var steamId = donation.Name?.Trim();
                if (string.IsNullOrEmpty(steamId))
                {
                    var msg = $"[DonatikService] SteamID missing in 'Name' field for donation {donation.Id}.";
                    _logger.LogWarning(msg);
                    return (false, msg);
                }

                var user = await context.Users.FirstOrDefaultAsync(u => u.SteamID == steamId);
                if (user == null)
                {
                    var msg = $"[DonatikService] Skipped {donation.Id}. Reason: User not found (SteamID: {steamId}).";
                    _logger.LogWarning(msg);
                    return (false, msg);
                }

                // Credit Balance
                var status = "Completed(Sync)"; // MaxLength(20)
                var transaction = new Shared.WalletTransaction
                {
                    AppUserId = user.Id,
                    Amount = donation.Amount,
                    Date = DateTime.UtcNow,
                    ExternalTransactionId = donation.Id,
                    Status = status
                };

                user.Balance += donation.Amount;

                context.WalletTransactions.Add(transaction);
                await context.SaveChangesAsync();

                var successMsg = $"[DonatikService] Successfully credited {donation.Amount} {donation.Currency} to user {user.Username} ({steamId}).";
                _logger.LogInformation(successMsg);
                return (true, successMsg);
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException?.Message ?? ex.Message;
                var errorMsg = $"[DonatikService] Error processing donation {donation.Id}: {innerMsg}";
                _logger.LogError(ex, errorMsg);
                return (false, errorMsg); // Return inner exception for better debugging
            }
        }
    }
}
