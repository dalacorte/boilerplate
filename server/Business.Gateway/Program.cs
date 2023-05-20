using Ocelot.DependencyInjection;
using Ocelot.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json")
    .AddEnvironmentVariables();

builder.Services.AddOcelot();

WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

await app.UseOcelot();

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Gateway Working");
});

app.Run();