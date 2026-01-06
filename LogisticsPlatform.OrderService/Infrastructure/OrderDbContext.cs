using Microsoft.EntityFrameworkCore;
using LogisticsPlatform.OrderService.Domain;

namespace LogisticsPlatform.OrderService.Infrastructure;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración de la cabecera
        modelBuilder.Entity<Order>().HasKey(o => o.Id);
        
        // Configuración de la relación 1:N con borrado en cascada
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}