using LogisticsPlatform.CatalogService.Infrastructure;
using LogisticsPlatform.CatalogService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Infraestructura (Aspire inyecta las cadenas de conexión aquí)
builder.AddNpgsqlDbContext<CatalogDbContext>("catalogdb");
builder.AddRedisClient("redis");

// Registrar nuestro servicio de lógica de negocio
builder.Services.AddScoped<ProductService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

// Inicialización de BD automática
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();