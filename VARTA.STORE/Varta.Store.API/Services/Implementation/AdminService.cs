using Microsoft.EntityFrameworkCore;
using Varta.Store.API.Data;
using Varta.Store.API.Services.Interfaces;
using Varta.Store.Shared.DTO;

namespace Varta.Store.API.Services.Implementation;

public class AdminService : IAdminService
{
    private readonly StoreDbContext _context;

    public AdminService(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserProfileDto>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var users = await _context.Users
            .AsNoTracking()
            .Select(u => new UserProfileDto
            {
                Id = u.Id,
                Username = u.Username,
                SteamID = u.SteamID,
                AvatarUrl = u.AvatarUrl,
                Balance = u.Balance
            })
            .ToListAsync(ct);

        return users;
    }

    public async Task<UserProfileDto?> GetUserByIdAsync(int id, CancellationToken ct = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Orders)
                .ThenInclude(o => o.Items)
            .Include(u => u.Orders)
                .ThenInclude(o => o.OrderStatus)
            .Include(u => u.WalletTransactions)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user == null)
            return null;

        var dto = new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            SteamID = user.SteamID,
            AvatarUrl = user.AvatarUrl,
            Balance = user.Balance,
            Orders = user.Orders.Select(o => new OrderDto
            {
                Id = o.Id,
                DateCreated = o.DateCreated,
                TotalAmount = o.TotalAmount,
                StatusName = o.OrderStatus?.Name ?? "Unknown",
                Items = o.Items.Select(oi => new OrderProductDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    ImageUrl = oi.ImageUrl,
                    PriceAtPurchase = oi.PriceAtPurchase,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList(),
            Transactions = user.WalletTransactions.Select(t => new WalletTransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Date = t.Date,
                Status = t.Status,
                ExternalTransactionId = t.ExternalTransactionId ?? string.Empty
            }).ToList()
        };

        return dto;
    }
}
