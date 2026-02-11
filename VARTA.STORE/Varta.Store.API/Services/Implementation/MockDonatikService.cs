using Varta.Store.API.Services.Interfaces;
using Varta.Store.API.Services.Models;

namespace Varta.Store.API.Services.Implementation;

public class MockDonatikService : IDonatikService
{
    private readonly ILogger<MockDonatikService> _logger;

    public MockDonatikService(ILogger<MockDonatikService> logger)
    {
        _logger = logger;
    }

    public Task<List<DonatikDonation>> GetRecentDonationsAsync(int limit = 500, string? filterName = null)
    {
        _logger.LogWarning("MOCK DONATIK SERVICE: Returning fake donations for testing.");

        var donations = new List<DonatikDonation>
        {
             new DonatikDonation
             {
                 Id = Guid.NewGuid().ToString(),
                 Name = "76561197960287930", // Common default steamID or similar
                 Payment = new DonatikPayment { Amount = "100", Currency = "UAH" },
                 CreatedAt = DateTime.UtcNow,
                 Message = "Test Donation"
             },
             new DonatikDonation
             {
                 Id = "MOCK_PAYMENT_" + Random.Shared.Next(1000, 9999),
                 Name = "MockSteamID",
                 Payment = new DonatikPayment { Amount = "50", Currency = "UAH" },
                 CreatedAt = DateTime.UtcNow
             }
        };

        // If a filter is provided (e.g. current user's SteamID), generate a matching donation!
        if (!string.IsNullOrEmpty(filterName))
        {
            donations.Add(new DonatikDonation
            {
                Id = "MOCK_PAYMENT_" + Guid.NewGuid().ToString().Substring(0, 8),
                Name = filterName,
                Payment = new DonatikPayment { Amount = "1000", Currency = "UAH" },
                CreatedAt = DateTime.UtcNow,
                Message = "Auto-Generated Mock Donation"
            });
        }

        return Task.FromResult(donations);
    }

    public Task<(bool Success, string Message)> ProcessDonationAsync(DonatikDonation donation)
    {
        // Mock processing always succeeds
        return Task.FromResult((true, "Mock success"));
    }
}
