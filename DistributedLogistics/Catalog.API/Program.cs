using Catalog.API;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar ServiceDefaults (Observabilidad, HealthChecks)
builder.AddServiceDefaults();

// 2. Agregar soporte para PostgreSQL y Redis usando las claves de conexión de Aspire
// "catalogdb" es el nombre que definiremos en el AppHost
builder.AddNpgsqlDbContext<CatalogDbContext>("catalogdb");

// "cache" es el nombre del recurso Redis en el AppHost
builder.AddRedisClient("cache");

var app = builder.Build();

// 3. Mapear endpoints por defecto
app.MapDefaultEndpoints();

// 4. Migración automática de DB al iniciar (Solo para Dev)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    db.Database.EnsureCreated();
}

// 5. Definir Endpoints de la API (Minimal API)
app.MapGet("/products", async (CatalogDbContext db) => 
{
    return await db.Products.ToListAsync();
});

app.MapGet("/products/{id}", async (int id, CatalogDbContext db) => 
{
    return await db.Products.FindAsync(id) is Product product 
        ? Results.Ok(product) 
        : Results.NotFound();
});

app.Run();