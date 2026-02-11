using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varta.Store.API.Data;
using Varta.Store.Shared;

namespace Varta.Store.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServerTagsController : ControllerBase
{
    private readonly StoreDbContext _context;
    private readonly IConfiguration _configuration;

    public ServerTagsController(StoreDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<List<ServerTag>>> GetServerTags(CancellationToken ct)
    {
        var serverTags = await _context.ServerTags.ToListAsync(ct);
        return Ok(serverTags);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServerTag>> GetServerTag(int id, CancellationToken ct)
    {
        var serverTag = await _context.ServerTags.FindAsync(new object[] { id }, ct);

        if (serverTag == null)
        {
            return NotFound();
        }

        return Ok(serverTag);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServerTag>> CreateServerTag(ServerTag serverTag, CancellationToken ct)
    {
        _context.ServerTags.Add(serverTag);
        await _context.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetServerTag), new { id = serverTag.Id }, serverTag);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateServerTag(int id, ServerTag serverTag, CancellationToken ct)
    {
        if (id != serverTag.Id)
        {
            return BadRequest();
        }

        _context.Entry(serverTag).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ServerTagExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteServerTag(int id, CancellationToken ct)
    {
        var serverTag = await _context.ServerTags.FindAsync(new object[] { id }, ct);
        if (serverTag == null)
        {
            return NotFound();
        }

        _context.ServerTags.Remove(serverTag);
        await _context.SaveChangesAsync(ct);

        return NoContent();
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
                // Fallback default (Local development)
                var apiRoot = Directory.GetCurrentDirectory();
                // Save to wwwroot/images/products/servers to match the volume structure if mimicking production
                // OR just wwwroot/images/servers if we want to follow the local structure
                // But to be consistent with the fix:
                targetFolder = Path.Combine(Directory.GetParent(apiRoot)?.FullName ?? "", "Varta.Store.Client", "wwwroot", "images", "products", "servers");
            }
            else
            {
                // Docker Production: Storage:ImagesPath is "/app/images/products" (the volume root)
                // We MUST save inside this volume for the client to see it.
                var baseImagesPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), imagesPath));
                targetFolder = Path.Combine(baseImagesPath, "servers");
            }

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            var fileName = $"srv_{Guid.NewGuid().ToString("N")[..8]}_{file.FileName}";
            var filePath = Path.Combine(targetFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            // Return path relative to "images" folder as expected by client src="images/..."
            // If we saved to images/products/servers/..., and client prepends "images/",
            // we should return "products/servers/filename".
            return Ok(new { imageUrl = $"products/servers/{fileName}" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Помилка завантаження: {ex.Message}");
        }
    }

    private bool ServerTagExists(int id)
    {
        return _context.ServerTags.Any(e => e.Id == id);
    }
}
