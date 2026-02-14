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
            .Where(o => o.AppUserId == user.Id && o.StatusId == 2)
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
            .Where(o => o.AppUserId == user.Id && o.StatusId == 2)
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

    /// <summary>
    /// Claim a purchased product (mark as issued/seen).
    /// Handles both single-item orders and mixed orders by splitting if necessary.
    /// </summary>
    [HttpPost("claim")]
    public async Task<IActionResult> ClaimProduct([FromBody] GameClaimRequestDto request)
    {
        if (request.Quantity <= 0)
        {
            return BadRequest(new { error = "Quantity must be greater than 0" });
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.SteamID == request.SteamId);

        if (user == null)
        {
            return NotFound(new { error = "User not found", request.SteamId });
        }

        // Find the oldest PAID order containing this product
        // We look for 'StatusId == 2' (Paid)
        var targetOrder = await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.AppUserId == user.Id && o.StatusId == 2)
            .Where(o => o.Items.Any(i => i.ProductId == request.ProductId))
            .OrderBy(o => o.DateCreated)
            .FirstOrDefaultAsync();

        if (targetOrder == null)
        {
            return BadRequest(new { error = "No suitable paid order found for this product." });
        }

        var orderItem = targetOrder.Items.First(i => i.ProductId == request.ProductId);

        // Check if user is claiming MORE than available
        if (request.Quantity > orderItem.Quantity)
        {
            return BadRequest(new
            {
                error = "Requested quantity exceeds available quantity in the order.",
                availableQuantity = orderItem.Quantity,
                requestedQuantity = request.Quantity
            });
        }

        // Logic Branch:
        // A) Full Claim: process normally (move entire item)
        // B) Partial Claim: split the line item

        bool isPartialClaim = request.Quantity < orderItem.Quantity;
        bool isMixedOrder = targetOrder.Items.Count > 1;

        // Create new "Issued" order
        var newOrder = new Varta.Store.Shared.Order
        {
            AppUserId = user.Id,
            StatusId = 3, // Issued/Seen
            DateCreated = DateTime.UtcNow,
            TotalAmount = 0
        };

        if (isPartialClaim)
        {
            // Case: Partial Quantity Claim
            // 1. Create new item for the new order with CLAIMED quantity
            var newItem = new Varta.Store.Shared.OrderProductHistory
            {
                ProductId = orderItem.ProductId,
                ProductName = orderItem.ProductName,
                ImageUrl = orderItem.ImageUrl,
                PriceAtPurchase = orderItem.PriceAtPurchase,
                Quantity = request.Quantity, // Only the claimed amount
                Order = newOrder
            };
            newOrder.Items.Add(newItem);

            // 2. Decrement quantity in old order
            orderItem.Quantity -= request.Quantity;

            // 3. Recalculate totals
            decimal claimedTotal = orderItem.PriceAtPurchase * request.Quantity;

            newOrder.TotalAmount = claimedTotal;
            targetOrder.TotalAmount -= claimedTotal;

            if (targetOrder.TotalAmount < 0) targetOrder.TotalAmount = 0; // Safety

            _context.Orders.Add(newOrder);
            // targetOrder remains valid with reduced quantity
        }
        else
        {
            // Case: Full Claim (Request.Quantity == orderItem.Quantity)

            if (isMixedOrder)
            {
                // Full claim of one item from a mixed basket -> Move the item
                var newItem = new Varta.Store.Shared.OrderProductHistory
                {
                    ProductId = orderItem.ProductId,
                    ProductName = orderItem.ProductName,
                    ImageUrl = orderItem.ImageUrl,
                    PriceAtPurchase = orderItem.PriceAtPurchase,
                    Quantity = orderItem.Quantity,
                    Order = newOrder
                };
                newOrder.Items.Add(newItem);

                // Remove from old order
                _context.OrderProductHistory.Remove(orderItem);

                // Recalc
                decimal itemTotal = orderItem.PriceAtPurchase * orderItem.Quantity;
                newOrder.TotalAmount = itemTotal;
                targetOrder.TotalAmount -= itemTotal;
                if (targetOrder.TotalAmount < 0) targetOrder.TotalAmount = 0;

                _context.Orders.Add(newOrder);
            }
            else
            {
                // Full claim of the ONLY item in the order -> Just flip status
                // We discard newOrder here
                targetOrder.StatusId = 3; // Issued/Seen
            }
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Game API: Claimed {Quantity}x Product {ProductId} for {SteamId}. Partial: {Partial}. Mixed: {Mixed}",
            request.Quantity, request.ProductId, request.SteamId, isPartialClaim, isMixedOrder);

        return Ok(new { success = true, message = "Item claimed successfully." });
    }
}
