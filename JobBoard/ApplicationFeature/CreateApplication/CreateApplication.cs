using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.ApplicationFeature.CreateApplication;

public record CreateApplicationDto(string Description, string JobId);

public class CreateApplication : IEndpointMarker
{
    public void Register(IEndpointRouteBuilder app)
     => app.MapGroup("api")
     .MapPost("application", ([FromBody] CreateApplicationDto dto, CancellationToken cancellationToken) =>
     {

     })
     .RequireAuthorization("ApplicantOnly");
}
