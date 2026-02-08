using Microsoft.EntityFrameworkCore;
using Varta.Store.API.Data;
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
                CategoryName = p.Category.Name,
                ServerTagName = p.ServerTag.Name
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
                CategoryName = p.Category.Name,
                ServerTagName = p.ServerTag.Name
            })
            .FirstOrDefaultAsync(ct);
    }
}