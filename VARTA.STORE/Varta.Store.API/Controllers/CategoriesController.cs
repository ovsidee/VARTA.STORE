using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varta.Store.API.Data;
using Varta.Store.Shared;

namespace Varta.Store.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly StoreDbContext _context;

    public CategoriesController(StoreDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Category>>> GetCategories(CancellationToken ct)
    {
        var categories = await _context.Categories
            .Select(c => new Category { Id = c.Id, Name = c.Name })
            .ToListAsync(ct);

        return Ok(categories);
    }
}
