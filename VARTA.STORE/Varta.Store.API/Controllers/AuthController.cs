using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Varta.Store.API.Data;
using Varta.Store.Shared;

namespace Varta.Store.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly StoreDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(StoreDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/api/auth/callback"
        }, "Steam");
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback()
    {
        //data from steam
        var result = await HttpContext.AuthenticateAsync("Cookies");
        if (!result.Succeeded) return BadRequest("Steam auth failed");

        var claims = result.Principal.Claims;

        //getting steamId (after link steam id present)
        var steamIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var steamId = steamIdClaim?.Split('/').Last();
        
        var username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Survivor";
        var avatarUrl = claims.FirstOrDefault(c => c.Type == "urn:steam:avatar:full")?.Value ?? "";

        if (string.IsNullOrEmpty(steamId)) return BadRequest("Steam ID not found");

        // creating user in db
        var user = await _context.Users.FirstOrDefaultAsync(u => u.SteamID == steamId);

        if (user == null)
        {
            user = new AppUser
            {
                SteamID = steamId,
                Username = username,
                AvatarUrl = avatarUrl,
                ProfileUrl = steamIdClaim ?? "",
                Balance = 0,
                DateCreated = DateTime.UtcNow
            };
            _context.Users.Add(user);
        }
        else
        {
            // if user already exists update data
            user.Username = username;
            user.AvatarUrl = avatarUrl;
        }

        await _context.SaveChangesAsync();

        // jwt
        var token = GenerateJwtToken(user);

        // redirect to home page
        return Redirect($"https://localhost:7120/login-success?token={token}");
    }

    private string GenerateJwtToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("SteamId", user.SteamID),
            new Claim("AvatarUrl", user.AvatarUrl),
            new Claim("Balance", user.Balance.ToString("F2")),
            new Claim(ClaimTypes.Role, "User")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}