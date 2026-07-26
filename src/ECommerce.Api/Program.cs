using ECommerce.Application;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>(tags: new[] { "ready" });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Liveness probe (basic app responsiveness)
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false
});

// Readiness probe (includes infrastructure dependencies)
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.Run();

public partial class Program { }
