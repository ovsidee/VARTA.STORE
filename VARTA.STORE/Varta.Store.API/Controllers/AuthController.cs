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
        // Бібліотека сама побудує правильний Realm завдяки налаштуванням Nginx
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/api/auth/callback"
        }, "Steam");
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback()
    {
        // Отримуємо дані від Steam через бібліотеку
        var result = await HttpContext.AuthenticateAsync("Cookies");
        
        if (!result.Succeeded) 
            return BadRequest("Steam auth failed. Result was not succeeded.");

        // Отримуємо Claims
        var claims = result.Principal.Claims;
        var steamIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        
        // Steam повертає ID у форматі: https://steamcommunity.com/openid/id/76561198...
        var steamId = steamIdClaim?.Split('/').Last();

        if (string.IsNullOrEmpty(steamId)) return BadRequest("Steam ID not found");

        // --- ДАЛЕЕ ВАШ СТАНДАРТНЫЙ КОД ПОЛУЧЕНИЯ АВАТАРА ---
        var apiKey = _configuration["Authentication:Steam:ApplicationKey"];
        var username = "Survivor";
        var avatarUrl = "";

        using (var httpClient = new HttpClient())
        {
            try 
            {
                var response = await httpClient.GetFromJsonAsync<SteamWebApiResponse>(
                    $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={steamId}");
                var player = response?.Response?.Players?.FirstOrDefault();
                if (player != null) { username = player.PersonaName; avatarUrl = player.AvatarFull; }
            }
            catch {}
        }
        
        // --- РАБОТА С БАЗОЙ ДАННЫХ ---
        var user = await _context.Users.FirstOrDefaultAsync(u => u.SteamID == steamId);
        if (user == null)
        {
            user = new AppUser
            {
                SteamID = steamId, Username = username, AvatarUrl = avatarUrl,
                ProfileUrl = $"https://steamcommunity.com/profiles/{steamId}",
                Balance = 0, DateCreated = DateTime.UtcNow
            };
            _context.Users.Add(user);
        }
        else
        {
            user.Username = username;
            user.AvatarUrl = avatarUrl;
        }
        await _context.SaveChangesAsync();

        // --- ГЕНЕРАЦИЯ JWT ---
        var token = GenerateJwtToken(user);

        // Редирект на фронтенд
        var clientUrl = _configuration["AppUrl"] ?? "http://localhost:7120";
        clientUrl = clientUrl.TrimEnd('/');
        
        return Redirect($"{clientUrl}/login-success?token={token}");
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

public class SteamWebApiResponse { public SteamResponse Response { get; set; } }
public class SteamResponse { public List<SteamPlayer> Players { get; set; } }
public class SteamPlayer 
{ 
    [System.Text.Json.Serialization.JsonPropertyName("personaname")]
    public string PersonaName { get; set; } 
    [System.Text.Json.Serialization.JsonPropertyName("avatarfull")]
    public string AvatarFull { get; set; } 
}