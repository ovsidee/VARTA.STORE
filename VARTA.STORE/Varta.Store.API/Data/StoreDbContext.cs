using Microsoft.EntityFrameworkCore;
using Varta.Store.Shared; // Refers to your Shared project where 'Product' is

namespace Varta.Store.API.Data
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
        {
        }
        
        public DbSet<Product> Products { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "M4A1", Price = 10.00m, ImageUrl = "m4a1.png" },
                new Product { Id = 2, Name = "Canned Bacon", Price = 2.50m, ImageUrl = "bacon.png" },
                new Product { Id = 3, Name = "Mountain Backpack", Price = 15.00m, ImageUrl = "backpack.png" }
            );
        }
    }
}