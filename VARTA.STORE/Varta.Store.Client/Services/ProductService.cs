using System.Net.Http.Json;
using Varta.Store.Shared.DTO;

namespace Varta.Store.Client.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _http;

    public ProductService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ProductDto>> GetAllProducts()
    {
        var result = await _http.GetFromJsonAsync<List<ProductDto>>("api/products");
        return result ?? new List<ProductDto>();
    }
}