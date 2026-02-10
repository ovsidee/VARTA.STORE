using Varta.Store.API.Enums;
using Varta.Store.Shared.DTO;

namespace Varta.Store.API.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken);
    Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken);
    Task<ProductStatusEnum> UpdateProductByIdAsync(int id, ProductUpdateDto dto, CancellationToken cancellationToken);
    Task<ProductUpdateDto> CreateProductAsync(ProductUpdateDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteProductByIdAsync(int id, CancellationToken cancellationToken);
}