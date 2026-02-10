using System.Net.Http.Json;
using Varta.Store.Shared.DTO;

namespace Varta.Store.Client.Services;

public class ProfileService : IProfileService
{
    private readonly HttpClient _http;

    public ProfileService(HttpClient http)
    {
        _http = http;
    }

    public async Task<UserProfileDto?> GetProfileAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<UserProfileDto>("api/profile");
        }
        catch (Exception)
        {
            // Handle error or return null
            return null;
        }
    }
}
