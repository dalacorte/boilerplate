using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks()
    .AddRedis(
        builder.Configuration.GetConnectionString("BaseRedis"),
        name: "Redis",
        failureStatus: HealthStatus.Degraded,
        tags: new string[] { "db", "data", "cache" })
    .AddMongoDb(
        builder.Configuration.GetConnectionString("BaseMongoDB"),
        name: "MongoDB",
        failureStatus: HealthStatus.Degraded,
        tags: new string[] { "db", "data", "nosql" })
    .AddUrlGroup(
        new Uri("http://localhost:5000/status"),
        httpMethod: HttpMethod.Get,
        name: "API",
        failureStatus: HealthStatus.Degraded,
        tags: new string[] { "url", "rest", "microservice", "aspnet" })
    .AddUrlGroup(
        new Uri("http://localhost:5000/status"),
        httpMethod: HttpMethod.Get,
        name: "Gateway",
        failureStatus: HealthStatus.Degraded,
        tags: new string[] { "url", "microservice" })
    .AddDiskStorageHealthCheck(x =>
        x.AddDrive("C:\\", 10_000),
        name: "Disk",
        failureStatus: HealthStatus.Degraded,
        tags: new string[] { "disk", "storage" });

builder.Services.AddHealthChecksUI(options =>
{
    options.SetEvaluationTimeInSeconds(5);
    options.MaximumHistoryEntriesPerEndpoint(10);
}).AddInMemoryStorage();

WebApplication app = builder.Build();

app.UseHealthChecks("/healthchecks-data", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHealthChecksUI(options =>
{
    options.UIPath = "/dashboard";
    options.AddCustomStylesheet("Style/health-check.css");
});

app.Run();