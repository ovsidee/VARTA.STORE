using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varta.Store.API.Data;
using Varta.Store.API.Services.Models;
using Varta.Store.Shared;

namespace Varta.Store.API.Controllers;

[ApiController]
[Route("api/webhook/donatik")]
public class DonatikWebhookController : ControllerBase
{
    private readonly StoreDbContext _context;
    private readonly ILogger<DonatikWebhookController> _logger;
    private readonly IConfiguration _configuration;
    private readonly Services.Interfaces.IDonatikService _donatikService;

    public DonatikWebhookController(StoreDbContext context, ILogger<DonatikWebhookController> logger, IConfiguration configuration, Services.Interfaces.IDonatikService donatikService)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
        _donatikService = donatikService;
    }

    [HttpGet("sync")]
    public async Task<IActionResult> SyncMissedDonations()
    {
        try
        {
            _logger.LogInformation("[DonatikWebhook] Starting manual sync of recent donations...");
            var donations = await _donatikService.GetRecentDonationsAsync(100); // Fetch last 100

            var results = new List<object>();
            int processedCount = 0;

            foreach (var donation in donations)
            {
                var result = await _donatikService.ProcessDonationAsync(donation);
                if (result.Success) processedCount++;

                results.Add(new
                {
                    Id = donation.Id,
                    Amount = donation.Amount,
                    Status = result.Success ? "Success" : $"Failed: {result.Message}"
                });
            }

            return Ok(new
            {
                message = $"Sync completed. Processed {processedCount} NEW donations.",
                details = results
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[DonatikWebhook] Sync failed.");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("debug")]
    public async Task<IActionResult> DebugDonations()
    {
        try
        {
            var donations = await _donatikService.GetRecentDonationsAsync(100);
            var results = new List<object>();

            foreach (var d in donations)
            {
                var steamId = d.Name?.Trim();
                var exists = await _context.WalletTransactions.AnyAsync(t => t.ExternalTransactionId == d.Id);
                var userExists = !string.IsNullOrEmpty(steamId) && await _context.Users.AnyAsync(u => u.SteamID == steamId);

                results.Add(new
                {
                    Id = d.Id,
                    Amount = d.Amount,
                    Name = d.Name,
                    Status = exists ? "Already Processed" : (userExists ? "Ready to Process" : "User Not Found"),
                    InternalReason = exists ? "Transaction ID exists in DB" : (userExists ? "None" : $"No user with SteamID '{steamId}'")
                });
            }

            return Ok(results);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> HandleWebhook([FromBody] DonatikDonation donation)
    {
        try
        {
            _logger.LogInformation($"[DonatikWebhook] Received webhook for Donation ID: {donation.Id}. Amount: {donation.Amount} {donation.Currency}. User: {donation.Name}");

            var result = await _donatikService.ProcessDonationAsync(donation);

            if (result.Success)
            {
                return Ok(); // Credited
            }
            else
            {
                // Either duplicate, user missing, or error. 
                // We return Ok() to acknowledge receipt (idempotency).
                _logger.LogWarning($"[DonatikWebhook] Webhook failed for {donation.Id}: {result.Message}");
                return Ok();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[DonatikWebhook] Error processing webhook.");
            return StatusCode(500, "Internal Server Error");
        }
    }
}
