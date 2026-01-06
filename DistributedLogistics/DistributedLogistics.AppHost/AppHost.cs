var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.DistributedLogistics_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.DistributedLogistics_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
