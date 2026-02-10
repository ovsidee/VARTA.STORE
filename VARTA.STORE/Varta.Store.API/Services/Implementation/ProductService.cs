using Microsoft.EntityFrameworkCore;
using Varta.Store.API.Data;
using Varta.Store.API.Enums;
using Varta.Store.API.Mappers;
using Varta.Store.API.Services.Interfaces;
using Varta.Store.Shared.DTO;

namespace Varta.Store.API.Services.Implementation;

public class ProductService : IProductService
{
    private readonly StoreDbContext _context;

    public ProductService(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> GetAllProductsAsync(CancellationToken ct)
    {
        return await _context.Products
            .AsNoTracking() // boost for read-only lists
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                ServerTagName = p.ServerTag != null ? p.ServerTag.Name : string.Empty
            })
            .ToListAsync(ct);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                ServerTagName = p.ServerTag != null ? p.ServerTag.Name : string.Empty
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<ProductStatusEnum> UpdateProductByIdAsync(int id, ProductUpdateDto dto, CancellationToken ct)
    {
        if (dto.Price <= 0 || string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Description))
            return ProductStatusEnum.VALIDATION_FAILED;

        var resultProduct = await _context
            .Products
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (resultProduct is null) return ProductStatusEnum.NOT_FOUND;

        resultProduct.Name = dto.Name;
        resultProduct.Description = dto.Description;
        resultProduct.Price = dto.Price;
        resultProduct.ImageUrl = dto.ImageUrl;
        resultProduct.CategoryId = dto.CategoryId;
        resultProduct.ServerTagId = dto.ServerTagId;

        await _context.SaveChangesAsync(ct);

        return ProductStatusEnum.SUCCESS;
    }

    public async Task<ProductUpdateDto> CreateProductAsync(ProductUpdateDto dto, CancellationToken cancellationToken)
    {
        var productToInsert = ProductMapper.FromDto(dto);

        await _context.AddAsync(productToInsert, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ProductMapper.ToDto(productToInsert);
    }

    public async Task<bool> DeleteProductByIdAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _context
            .Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (result == null)
            return false;

        _context.Products.Remove(result);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}