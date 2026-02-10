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

    public ProductsController(IProductService productService)
    {
        _productService = productService;
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

}