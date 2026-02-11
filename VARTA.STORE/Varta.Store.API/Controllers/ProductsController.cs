using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Varta.Store.API.Enums;
using Varta.Store.API.Services.Interfaces;
using Varta.Store.Shared.DTO;

namespace Varta.Store.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IConfiguration _configuration;

    public ProductsController(IProductService productService, IConfiguration configuration)
    {
        _productService = productService;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts(CancellationToken ct)
    {
        var products = await _productService.GetAllProductsAsync(ct);

        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id, CancellationToken ct)
    {
        var product = await _productService.GetProductByIdAsync(id, ct);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProductAsync(int id, [FromBody] ProductUpdateDto dto, CancellationToken ct)
    {
        var result = await _productService.UpdateProductByIdAsync(id, dto, ct);

        if (result == ProductStatusEnum.VALIDATION_FAILED)
            return BadRequest();

        if (result == ProductStatusEnum.NOT_FOUND)
            return NotFound();

        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateProductAsync(ProductUpdateDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Description) ||
            string.IsNullOrEmpty(dto.ImageUrl) || dto.Price <= 0 || dto.CategoryId < 0 || dto.ServerTagId < 0)
            return BadRequest();

        var result = await _productService.CreateProductAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProductAsync(int id, CancellationToken cancellationToken)
    {
        if (id < 0)
            return BadRequest();

        var result = await _productService.DeleteProductByIdAsync(id, cancellationToken);

        if (result) return Ok();
        return NotFound();
    }

    [HttpPost("upload-image")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не обрано");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

        if (!allowedExtensions.Contains(extension))
            return BadRequest("Недопустимий формат файлу. Дозволені: .jpg, .jpeg, .png, .webp, .gif");

        if (file.Length > 5 * 1024 * 1024) // 5MB
            return BadRequest("Розмір файлу перевищує 5MB");

        try
        {
            var imagesPath = _configuration["Storage:ImagesPath"];
            string targetFolder;

            if (string.IsNullOrEmpty(imagesPath))
            {
                // Fallback default
                var apiRoot = Directory.GetCurrentDirectory();
                targetFolder = Path.Combine(Directory.GetParent(apiRoot)?.FullName ?? "", "Varta.Store.Client", "wwwroot", "images", "products");
            }
            else
            {
                // Handle both absolute paths (Docker) and relative paths (Dev)
                targetFolder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), imagesPath));
            }

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            var fileName = $"img_{Guid.NewGuid().ToString("N")[..8]}_{file.FileName}";
            var filePath = Path.Combine(targetFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            // Return relative path for DB
            return Ok(new { imageUrl = $"products/{fileName}" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Помилка завантаження: {ex.Message}");
        }
    }

}
