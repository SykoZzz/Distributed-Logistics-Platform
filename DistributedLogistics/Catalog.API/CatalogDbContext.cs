using Microsoft.EntityFrameworkCore;

namespace Catalog.API;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Seed data (Datos de prueba iniciales)
        builder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop Gamer", Description = "High end laptop", Price = 1500.00m, Stock = 10 },
            new Product { Id = 2, Name = "Mechanical Keyboard", Description = "Clicky keys", Price = 100.00m, Stock = 50 },
            new Product { Id = 3, Name = "Gaming Mouse", Description = "RGB lights", Price = 50.00m, Stock = 100 }
        );
    }
}