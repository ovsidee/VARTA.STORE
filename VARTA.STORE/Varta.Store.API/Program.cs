using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Varta.Store.API.Data; 
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Varta.Store.API.Services.Implementation;
using Varta.Store.API.Services.Interfaces;

namespace Varta.Store.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // add db (postgresql)
        builder.Services.AddDbContext<StoreDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        );
        
        var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        
        // DI for services
        builder.Services.AddScoped<IProductService, ProductService>();
        
        // add controllers
        builder.Services.AddControllers();
        
        builder.Services.AddEndpointsApiExplorer(); 
        
        builder.Services.AddOpenApi();

        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
        
        // cors
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("StrictPolicy", policy =>
            {
                if (allowedOrigins != null && allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
                else
                {
                    // block everything if config is missing
                    // or allow all ONLY in Dev mode
                    if (builder.Environment.IsDevelopment())
                    {
                        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                }
            });
        });
        
        builder.Services.AddAuthentication(options =>
            {
                // for protecting API endpoints
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie("Cookies") // Needed temporarily for the Steam handshake
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            })
            .AddSteam(options =>
            {
                // the temporary cookie with the Steam login
                options.SignInScheme = "Cookies"; 
                options.ApplicationKey = builder.Configuration["Authentication:Steam:ApplicationKey"];
            });

        var app = builder.Build();

        // db migration for docker
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            
            try 
            {
                var context = services.GetRequiredService<StoreDbContext>();
                
                // retry loop to wait for Postgres to wake up
                int retries = 5;
                while (retries > 0)
                {
                    try
                    {
                        context.Database.EnsureCreated(); // Creates DB if not exists
                        logger.LogInformation("Database connected!");
                        break;
                    }
                    catch (Exception)
                    {
                        retries--;
                        if (retries == 0) throw;
                        logger.LogWarning("Waiting for Database...");
                        Thread.Sleep(2000);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database migration failed.");
            }
        }
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi(); 
        }
        
        app.UseCors("StrictPolicy");        
        
        app.UseAuthentication();
        
        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}