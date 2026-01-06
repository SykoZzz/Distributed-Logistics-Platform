using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// -----------------------------------------------------------------------
// 1. INFRAESTRUCTURA (Contenedores)
// -----------------------------------------------------------------------

// Redis: Caché distribuido.
// Se asigna el nombre "redis" para inyección de dependencias.
var redis = builder.AddRedis("redis");

// RabbitMQ: Mensajería.
// .WithManagementPlugin() habilita la consola web en el puerto 15672.
var rabbitMq = builder.AddRabbitMQ("messaging")
                     .WithManagementPlugin();

// PostgreSQL: Base de datos.
// .WithPgAdmin() habilita la consola web para administrar la BD.
var postgresServer = builder.AddPostgres("postgres")
                            .WithPgAdmin();

// Definimos bases de datos lógicas separadas para mantener aislamiento.
var catalogDb = postgresServer.AddDatabase("catalogdb");
var orderDb = postgresServer.AddDatabase("orderdb");

// -----------------------------------------------------------------------
// 2. MICROSERVICIOS (APIs)
// -----------------------------------------------------------------------

// CatalogService: Usa Postgres y Redis.
var catalogService = builder.AddProject<Projects.LogisticsPlatform_CatalogService>("catalog-service")
                            .WithReference(catalogDb)
                            .WithReference(redis);

// OrderService: Usa Postgres y publica en RabbitMQ.
var orderService = builder.AddProject<Projects.LogisticsPlatform_OrderService>("order-service")
                          .WithReference(orderDb)
                          .WithReference(rabbitMq);

// ProcessorService: Worker que consume de RabbitMQ.
var processorService = builder.AddProject<Projects.LogisticsPlatform_ProcessorService>("processor-service")
                            .WithReference(rabbitMq);

// -----------------------------------------------------------------------
// 3. GATEWAY (Punto de Entrada)
// -----------------------------------------------------------------------

// ApiGateway: Recibe tráfico externo y lo redirige a los servicios internos.
builder.AddProject<Projects.LogisticsPlatform_ApiGateway>("api-gateway")
       .WithReference(catalogService)
       .WithReference(orderService)
       .WithReference(processorService)
       .WithExternalHttpEndpoints(); // Permite acceso desde fuera del clúster de Aspire

// Iniciar la orquestación
builder.Build().Run();