using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
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
                        .AllowAnyHeader()
                        .AllowCredentials(); 
                }
                else
                {
                    // Fallback for development if config is missing
                    policy.SetIsOriginAllowed(origin => true) // Allow any origin
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
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
                options.SignInScheme = "Cookies"; 
                options.ApplicationKey = builder.Configuration["Authentication:Steam:ApplicationKey"];
    
                // --- ДОДАЙТЕ ЦЕЙ РЯДОК ---
                // Це змусить Steam повертати юзера на /api/signin-steam
                // І Nginx перенаправить цей запит на бекенд!
                options.CallbackPath = "/api/signin-steam";
                // -------------------------

                options.Events.OnAuthenticated = context => {
                    return Task.CompletedTask;
                };
            });

        var app = builder.Build();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
        
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