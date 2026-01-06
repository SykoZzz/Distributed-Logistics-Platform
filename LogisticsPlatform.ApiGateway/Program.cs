var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(); // Métricas y HealthChecks

// Configurar YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver(); // ¡Clave para encontrar los otros servicios!

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapReverseProxy(); // Activa el proxy

app.Run();