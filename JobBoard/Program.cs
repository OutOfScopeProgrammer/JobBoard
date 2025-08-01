using JobBoard.Infrastructure.ExceptionHandlers;
using JobBoard.Infrastructure.Extensions;
using JobBoard.Infrastructure.Middlewares;
using JobBoard.Infrastructure.Persistence.RabbitMq.Producers;
using JobBoard.Infrastructure.Persistence.RabbitMq.Receiver;
using JobBoard.Shared.Persistence.Postgres;
using JobBoard.Shared.Persistence.Postgres.Seeder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogWithLokiConfiguration();

builder.Services.AddProjectDependecy(builder.Configuration);
builder.Services.AddRAbbitMq(builder.Configuration);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandler>();
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
app.MapGet("producer", async ([FromServices] EmailProducer producer) =>
{
    await producer.TestPublish("my email contet");
    return Results.Ok();
});
app.MapGet("receiver", async ([FromServices] EmailReceiver producer) =>
{
    await producer.TestReceiver();
    return Results.Ok();
});

app.Run();


