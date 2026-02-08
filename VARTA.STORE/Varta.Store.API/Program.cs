using Varta.Store.API.Data; 
using Microsoft.EntityFrameworkCore;
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
        
        // DI for services
        builder.Services.AddScoped<IProductService, ProductService>();
        
        // add controllers
        builder.Services.AddControllers();
        
        builder.Services.AddEndpointsApiExplorer(); 
        
        builder.Services.AddOpenApi();

        // cors
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorClient",
                policy =>
                {
                    policy.WithOrigins("http://localhost", "http://localhost:80") 
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
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
                        logger.LogInformation("✅ Database connected!");
                        break;
                    }
                    catch (Exception)
                    {
                        retries--;
                        if (retries == 0) throw;
                        logger.LogWarning("⏳ Waiting for Database...");
                        Thread.Sleep(2000);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Database migration failed.");
            }
        }
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi(); 
        }
        
        app.UseCors("AllowBlazorClient");
        
        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}