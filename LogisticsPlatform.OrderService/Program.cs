using LogisticsPlatform.OrderService.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Inyección de la BD de Órdenes ("orderdb" viene del AppHost)
builder.AddNpgsqlDbContext<OrderDbContext>("orderdb");

// Inyección de RabbitMQ (Lo usaremos en la siguiente fase, pero lo dejamos listo)
builder.AddRabbitMQClient("messaging");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

// Inicialización de BD
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    // Esto creará las tablas Orders y OrderItems en Postgres
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