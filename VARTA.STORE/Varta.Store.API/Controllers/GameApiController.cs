using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varta.Store.API.Auth;
using Varta.Store.API.Data;
using Varta.Store.Shared.DTO;

namespace Varta.Store.API.Controllers;

[ApiController]
[Route("api/game")]
[ApiKeyAuth]
public class GameApiController : ControllerBase
{
    private readonly StoreDbContext _context;
    private readonly ILogger<GameApiController> _logger;

    public GameApiController(StoreDbContext context, ILogger<GameApiController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all purchased products for a player by SteamID.
    /// Only includes orders with status Paid (2) or Issued (3).
    /// </summary>
    [HttpGet("purchases/{steamId}")]
    public async Task<IActionResult> GetPurchases(string steamId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.SteamID == steamId);

        if (user == null)
        {
            return NotFound(new { error = "User not found", steamId });
        }

        // Get all order items from paid/issued orders
        var purchasedItems = await _context.Orders
            .AsNoTracking()
            .Where(o => o.AppUserId == user.Id && (o.StatusId == 2 || o.StatusId == 3))
            .SelectMany(o => o.Items.Select(i => new
            {
                i.ProductId,
                i.ProductName,
                i.Quantity,
                PurchasedAt = o.DateCreated
            }))
            .ToListAsync();

        var response = new GamePurchasesResponse
        {
            SteamId = steamId,
            Username = user.Username,
            Products = purchasedItems.Select(i => new GamePurchasedProductDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                PurchasedAt = i.PurchasedAt
            }).ToList()
        };

        _logger.LogInformation("Game API: Fetched {Count} purchased items for SteamID {SteamId}",
            response.Products.Count, steamId);

        return Ok(response);
    }

    /// <summary>
    /// Check if a player owns a specific product by product name.
    /// Uses case-insensitive matching.
    /// </summary>
    [HttpGet("purchases/{steamId}/check/{productName}")]
    public async Task<IActionResult> CheckOwnership(string steamId, string productName)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.SteamID == steamId);

        if (user == null)
        {
            return NotFound(new { error = "User not found", steamId });
        }

        // Sum quantities across all paid/issued orders for this product name
        var totalQuantity = await _context.Orders
            .AsNoTracking()
            .Where(o => o.AppUserId == user.Id && (o.StatusId == 2 || o.StatusId == 3))
            .SelectMany(o => o.Items)
            .Where(i => i.ProductName.ToLower() == productName.ToLower())
            .SumAsync(i => i.Quantity);

        var response = new GameProductOwnershipResponse
        {
            SteamId = steamId,
            ProductName = productName,
            Owned = totalQuantity > 0,
            TotalQuantity = totalQuantity
        };

        _logger.LogInformation(
            "Game API: Ownership check for '{ProductName}' by SteamID {SteamId}: owned={Owned}, qty={Qty}",
            productName, steamId, response.Owned, totalQuantity);

        return Ok(response);
    }
}
