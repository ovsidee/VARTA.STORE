using System.Net.Http.Json;
using Varta.Store.Shared;

namespace Varta.Store.Client.Services;

public class ServerTagService : IServerTagService
{
    private readonly HttpClient _http;

    public ServerTagService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ServerTag>> GetServerTags()
    {
        var result = await _http.GetFromJsonAsync<List<ServerTag>>("api/servertags");
        return result ?? new List<ServerTag>();
    }
}
