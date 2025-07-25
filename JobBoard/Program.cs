using JobBoard.Infrastructure.Extensions;
using JobBoard.Infrastructure.Logger.Middlewares;
using JobBoard.Shared.ExceptionHandlers;
using JobBoard.Shared.Persistence;
using JobBoard.Shared.Persistence.Seeder;
using Scalar.AspNetCore;
using Serilog;



var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogWithLokiConfiguration();

builder.Services.AddProjectDependecy(builder.Configuration);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseGlobalExceptionhandler();
app.UseMiddleware<LogMiddleware>();

using var scope = app.Services.CreateScope();
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
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapApplicationEndpoints();

app.Run();


