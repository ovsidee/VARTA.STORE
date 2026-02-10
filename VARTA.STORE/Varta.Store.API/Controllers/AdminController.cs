using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varta.Store.API.Services.Interfaces;
using Varta.Store.Shared.DTO;

namespace Varta.Store.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<UserProfileDto>>> GetUsers(CancellationToken ct)
    {
        var users = await _adminService.GetAllUsersAsync(ct);
        return Ok(users);
    }

    [HttpGet("users/{id}")]
    public async Task<ActionResult<UserProfileDto>> GetUser(int id, CancellationToken ct)
    {
        var user = await _adminService.GetUserByIdAsync(id, ct);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}
