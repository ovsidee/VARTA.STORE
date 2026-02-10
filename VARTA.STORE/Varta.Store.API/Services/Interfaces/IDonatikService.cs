using Varta.Store.API.Services.Models;

namespace Varta.Store.API.Services.Interfaces;

public interface IDonatikService
{
    Task<List<DonatikDonation>> GetRecentDonationsAsync(int limit = 500, string? filterName = null);
}
