using Varta.Store.API.Data; 
using Microsoft.EntityFrameworkCore;

namespace Varta.Store.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Register the Database 
        builder.Services.AddDbContext<StoreDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        );

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer(); 
        builder.Services.AddOpenApi();

        // 2. Add CORS
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

        // 3. DATABASE MIGRATION & RETRY LOGIC (Crucial for Docker)
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            
            try 
            {
                var context = services.GetRequiredService<StoreDbContext>();
                
                // Retry loop to wait for Postgres to wake up
                int retries = 5;
                while (retries > 0)
                {
                    try
                    {
                        context.Database.EnsureCreated(); // Creates DB if not exists
                        // DbSeeder.Seed(context); // Uncomment if you created the Seeder class
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

        // 4. Configure Pipeline
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi(); // or UseSwagger()
        }
        
        app.UseCors("AllowBlazorClient");
        
        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}