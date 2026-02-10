using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Varta.Store.API.Data;
using Varta.Store.Shared;
using Varta.Store.Shared.DTO;

namespace Varta.Store.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly StoreDbContext _context;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(StoreDbContext context, ILogger<OrdersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto request)
    {
        if (request.Items == null || !request.Items.Any())
        {
            return BadRequest("Order items cannot be empty.");
        }

        var steamId = User.FindFirst("SteamId")?.Value;
        if (string.IsNullOrEmpty(steamId))
        {
            return Unauthorized();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.SteamID == steamId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        // 1. Validate Products & Calculate Total
        decimal totalAmount = 0;
        var orderItems = new List<OrderProductHistory>();

        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

        foreach (var item in request.Items)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product == null)
            {
                return BadRequest($"Product with ID {item.ProductId} not found.");
            }

            totalAmount += product.Price * item.Quantity;

            orderItems.Add(new OrderProductHistory
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = item.Quantity,
                PriceAtPurchase = product.Price,
                ImageUrl = product.ImageUrl
                // OrderId will be set after saving order
            });
        }

        // 2. Check Balance
        if (user.Balance < totalAmount)
        {
            return BadRequest($"Недостатньо коштів. Ваш баланс: {user.Balance} ₴, сума замовлення: {totalAmount} ₴");
        }

        // 3. Deduct Balance
        user.Balance -= totalAmount;

        // 4. Create Order
        // Default status: Paid (Since we just deducted money)
        // Or "Pending" if we need manual approval? 
        // User said "User buy... Product successfully added". So it's instant.
        // Let's find "Paid" status ID. Seed says 2 = Paid.

        var order = new Order
        {
            AppUserId = user.Id,
            TotalAmount = totalAmount,
            DateCreated = DateTime.UtcNow,
            StatusId = 2, // Paid
            Items = orderItems
        };

        _context.Orders.Add(order);

        // This implicitly saves OrderItems too because of the navigation property, 
        // but OrderProductHistory needs OrderId. EF Core handles this if relationship is configured correctly.
        // Looking at Order.cs: public List<OrderProductHistory> Items { get; set; }
        // Looking at OrderProductHistory.cs: probably has OrderId.

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Order {order.Id} created for user {user.Username}. Total: {totalAmount}");

        return Ok(new OrderDto
        {
            Id = order.Id,
            TotalAmount = totalAmount,
            StatusName = "Оплачено",
            DateCreated = order.DateCreated
        });
    }
}
