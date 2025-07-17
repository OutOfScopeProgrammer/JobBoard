using JobBoard.Shared.Extensions;
using JobBoard.Shared.Persistence;
using JobBoard.Shared.Persistence.Seeder;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProjectDependecy(builder.Configuration);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
var loggerFactor = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
Seeder.Initialize(db, loggerFactor);

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


