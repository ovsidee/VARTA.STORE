using Varta.Store.Shared.DTO;

namespace Varta.Store.API.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductDto>> GetAllProductsAsync(CancellationToken ct);
    Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken ct);
}