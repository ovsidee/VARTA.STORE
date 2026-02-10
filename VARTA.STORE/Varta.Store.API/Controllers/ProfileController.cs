using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varta.Store.API.Data;
using Varta.Store.Shared.DTO;

namespace Varta.Store.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly StoreDbContext _context;

    public ProfileController(StoreDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        // Get user ID from claims (set in AuthController)
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        // Fetch user with orders and related data
        var user = await _context.Users
            .Include(u => u.Orders)
                .ThenInclude(o => o.OrderStatus)
            .Include(u => u.Orders)
                .ThenInclude(o => o.Items)
                    .ThenInclude(oph => oph.Product)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Map to DTO
        var profileDto = new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            SteamID = user.SteamID,
            AvatarUrl = user.AvatarUrl,
            Balance = user.Balance,
            Orders = user.Orders.Select(o => new OrderDto
            {
                Id = o.Id,
                TotalAmount = o.TotalAmount,
                DateCreated = o.DateCreated,
                StatusName = o.OrderStatus?.Name ?? "Unknown",
                Items = o.Items.Select(i => new OrderProductDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "Unknown Product",
                    ImageUrl = i.Product?.ImageUrl ?? "",
                    PriceAtPurchase = i.PriceAtPurchase,
                    Quantity = i.Quantity
                }).ToList()
            }).OrderByDescending(o => o.DateCreated).ToList()
        };

        return Ok(profileDto);
    }
}
