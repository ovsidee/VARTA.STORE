using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Varta.Store.API.Data;
using Varta.Store.API.Services.Interfaces;
using Varta.Store.Shared;

namespace Varta.Store.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly StoreDbContext _context;
    private readonly IDonatikService _donatikService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(StoreDbContext context, IDonatikService donatikService, ILogger<PaymentController> logger)
    {
        _context = context;
        _donatikService = donatikService;
        _logger = logger;
    }

    [HttpPost("check")]
    public async Task<IActionResult> CheckPayment([FromBody] CheckPaymentRequest request)
    {
        // 1. Get current user
        var steamId = User.FindFirst("SteamId")?.Value;
        // Wait, looking at AppUser, we have a SteamID column. 
        // We need to match the user from DB.

        // The steam auth usually puts the numeric ID in the NameIdentifier claim.
        // "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" -> "76561198..."

        if (string.IsNullOrEmpty(steamId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.SteamID == steamId);
        if (user == null)
        {
            return NotFound("User profile not found.");
        }

        // 2. Fetch donations from Donatik
        // Pass the steamId as filterName to help the Mock service (and potentially real service if updated later)
        var donations = await _donatikService.GetRecentDonationsAsync(500, steamId);

        // 3. Find a matching donation
        // Logic:
        // - Name must match SteamID (case insensitive)
        // - Amount must be >= request.Amount
        // - Must NOT be already used (check WalletTransactions)

        // Get processed IDs from DB
        var processedIds = await _context.WalletTransactions
            .Select(t => t.ExternalTransactionId)
            .ToListAsync();

        var matchingDonation = donations.FirstOrDefault(d =>
            d.Name.Trim().Equals(steamId, StringComparison.OrdinalIgnoreCase) &&
            d.Amount >= request.Amount &&
            !processedIds.Contains(d.Id) // Ensure not already processed
        );

        if (matchingDonation == null)
        {
            return BadRequest("Payment not found or all matching payments have already been credited. Please try again in 1 minute.");
        }

        // 4. Check if already processed (Redundant, handled in step 3)
        // var alreadyProcessed = await _context.WalletTransactions...

        // 5. Process Transaction
        var transaction = new WalletTransaction
        {
            AppUserId = user.Id,
            Amount = matchingDonation.Amount,
            Date = DateTime.UtcNow,
            ExternalTransactionId = matchingDonation.Id,
            Status = "Completed"
        };

        user.Balance += matchingDonation.Amount;

        _context.WalletTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"User {user.Username} ({steamId}) topped up {matchingDonation.Amount} UAH. Transaction ID: {matchingDonation.Id}");

        return Ok(new
        {
            message = "Balance updated successfully!",
            newBalance = user.Balance,
            addedAmount = matchingDonation.Amount
        });
    }

    public class CheckPaymentRequest
    {
        public decimal Amount { get; set; }
    }
}
