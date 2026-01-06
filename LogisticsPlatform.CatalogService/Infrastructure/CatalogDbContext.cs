using Microsoft.EntityFrameworkCore;
using LogisticsPlatform.CatalogService.Domain;

namespace LogisticsPlatform.CatalogService.Infrastructure;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasKey(p => p.Id);
        
        // Seed Data para pruebas inmediatas
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop Pro", Price = 2500.00m, Stock = 10 },
            new Product { Id = 2, Name = "Auriculares Noise Cancelling", Price = 300.00m, Stock = 50 }
        );
    }
}