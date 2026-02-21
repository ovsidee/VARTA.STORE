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
        // Fix for "Cannot write DateTime with Kind=UTC to PostgreSQL type 'timestamp without time zone'"
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        // Load .env file manually
        var root = Directory.GetCurrentDirectory();
        var dotenv = Path.Combine(root, ".env");
        if (!File.Exists(dotenv))
        {
            // Try parent directory (solution root)
            dotenv = Path.Combine(Directory.GetParent(root)?.FullName ?? "", ".env");
        }

        if (File.Exists(dotenv))
        {
            foreach (var line in File.ReadAllLines(dotenv))
            {
                var parts = line.Split('=', 2);
                if (parts.Length != 2) continue;
                Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
            }
        }

        var builder = WebApplication.CreateBuilder(args);

        // DEBUG: Print Connection String (masked)
        var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
        var maskedConnStr = System.Text.RegularExpressions.Regex.Replace(connStr ?? "NULL", "Password=[^;]*", "Password=***");
        Console.WriteLine($"[DEBUG] RAW CONNECTION STRING: {maskedConnStr}");

        builder.Services.AddDbContext<StoreDbContext>(options =>
            options.UseNpgsql(connStr)
        );

        var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IAdminService, AdminService>();

        var useMock = builder.Configuration.GetValue<bool>("Donatik:UseMock");

        if (useMock)
        {
            builder.Services.AddScoped<IDonatikService, MockDonatikService>();
        }
        else
        {
            builder.Services.AddHttpClient<IDonatikService, DonatikService>();
        }

        builder.Services.AddHostedService<Varta.Store.API.Services.Background.DonatikSyncWorker>();

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApi();

        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

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
                    policy.SetIsOriginAllowed(origin => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }
            });
        });

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie("Cookies")
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
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
                options.CallbackPath = "/api/signin-steam";

                options.Events.OnAuthenticated = context =>
                {
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
                        context.Database.Migrate();
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