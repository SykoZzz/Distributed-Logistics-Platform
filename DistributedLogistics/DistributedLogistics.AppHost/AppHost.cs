var builder = DistributedApplication.CreateBuilder(args);

// 1. Definir contenedor de Redis
var cache = builder.AddRedis("cache");

// 2. Definir contenedor de PostgreSQL y la base de datos
var postgres = builder.AddPostgres("postgres")
                      .WithDataVolume(); // Persistir datos si reiniciamos contenedor

var catalogDb = postgres.AddDatabase("catalogdb");

// 3. Definir el proyecto Catalog API y pasarle las referencias
builder.AddProject<Projects.Catalog_API>("catalog-api")
       .WithReference(cache)
       .WithReference(catalogDb);

builder.Build().Run();