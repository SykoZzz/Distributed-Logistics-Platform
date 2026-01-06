var builder = DistributedApplication.CreateBuilder(args);

// --- Recursos Existentes ---
var cache = builder.AddRedis("cache");
var postgres = builder.AddPostgres("postgres").WithDataVolume();
var catalogDb = postgres.AddDatabase("catalogdb");

// --- NUEVO: RabbitMQ ---
// Creamos el contenedor de mensajería con persistencia
var messaging = builder.AddRabbitMQ("messaging")
                       .WithDataVolume()
                       .WithManagementPlugin(); // Habilita la UI de admin de RabbitMQ

// --- NUEVO: Base de datos para Orders ---
var orderingDb = postgres.AddDatabase("orderingdb");

// --- Proyectos ---
builder.AddProject<Projects.Catalog_API>("catalog-api")
       .WithReference(cache)
       .WithReference(catalogDb);

// Agregamos el Ordering API conectado a la DB y al Bus
builder.AddProject<Projects.Ordering_API>("ordering-api")
       .WithReference(orderingDb)
       .WithReference(messaging)
       .WaitFor(orderingDb);

builder.Build().Run();