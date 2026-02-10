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

    public ServerTagsController(StoreDbContext context)
    {
        _context = context;
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

    private bool ServerTagExists(int id)
    {
        return _context.ServerTags.Any(e => e.Id == id);
    }
}
