using Varta.Store.Shared.DTO;

namespace Varta.Store.Client.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetAllProducts();
}