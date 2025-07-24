using JobBoard.Infrastructure.Extensions;
using JobBoard.Shared.Exceptionhandlers;
using JobBoard.Shared.Persistence;
using JobBoard.Shared.Persistence.Seeder;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Grafana.Loki;




var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(static (context, service, config) =>
{
    config.MinimumLevel.Fatal()
        // all other system logs are disable 
        .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Warning)
        // Only Filters in EndpointFilters directory will be logged with information level
        .MinimumLevel.Override("JobBoard.Shared.EndpointFilters", LogEventLevel.Information)
        .WriteTo.Console()
        .WriteTo.GrafanaLoki(
           "http://localhost:3100",
            labels: [new LokiLabel() { Key = "App", Value = "LokiSerilogDemo" }],
            textFormatter: new RenderedCompactJsonFormatter()
           );
});

builder.Services.AddProjectDependecy(builder.Configuration);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
using var scope = app.Services.CreateScope();
app.UseGlobalExceptionhandler();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
Seeder.Initialize(db, logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapApplicationEndpoints();

app.Run();


