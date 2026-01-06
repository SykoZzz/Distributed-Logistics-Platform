using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ordering.API;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// 1. Configurar DB (Postgres)
builder.AddNpgsqlDbContext<OrderingDbContext>("orderingdb", settings => 
{
    // Opcional: Configuraciones extra
});

// 2. Configurar MassTransit con RabbitMQ y Outbox
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<OrderingDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(1);
        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("messaging");
        cfg.Host(connectionString);
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

// 3. Migración Automática
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
    db.Database.EnsureCreated();
}

// 4. Endpoint para crear Pedidos (CORREGIDO)
app.MapPost("/orders", async (OrderingDbContext db, IPublishEndpoint publishEndpoint) => 
{
    // A. Lógica de Negocio
    var order = new Order 
    { 
        CustomerId = "user-123", 
        TotalAmount = 99.99m 
    };
    db.Orders.Add(order);

    // B. Publicar Evento
    // --- CAMBIO AQUÍ: Usamos el record explícito, NO anónimo ---
    await publishEndpoint.Publish(new OrderCreated(order.Id, order.CreatedAt));

    // C. Guardar Cambios
    await db.SaveChangesAsync();

    return Results.Created($"/orders/{order.Id}", order);
});

app.Run();

// MassTransit OBLIGA a que los eventos tengan un namespace
namespace Ordering.API
{
    public record OrderCreated(Guid OrderId, DateTime CreatedAt);
}