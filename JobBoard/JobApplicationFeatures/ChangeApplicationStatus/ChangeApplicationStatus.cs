using JobBoard.Domain.Enums;
using JobBoard.Infrastructure.Auth;
using JobBoard.JobApplicationFeatures.Services;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.JobApplicationFeatures.ChangeApplicationStatus;

public record ApplicationStatusDto(Status Status);
public class ChangeApplicationStatus : IEndpointMarker
{
    public RouteHandlerBuilder Register(IEndpointRouteBuilder app)
     => app.MapGroup("api")
     .MapPost("application/{applicationId}",
     async ([FromRoute] Guid applicationId,
     [FromBody] ApplicationStatusDto dto, JobApplicationService service, CancellationToken cancellationToken) =>
     {
         var response = await service.ChangeApplicationStatus(applicationId, dto.Status, cancellationToken);
         if (!response.IsSuccess)
         {
             if (response.Errors.FirstOrDefault() == ErrorMessages.Internal)
                 return Results.InternalServerError(response.Errors);
             if (response.Errors.FirstOrDefault() == ErrorMessages.NotFound)
                 return Results.NotFound(response.Errors);
         }
         return Results.NoContent();
     })
     .WithTags("Application")
     .WithSummary("Change application status")
     .WithDescription("تغییر وضعیت رزومه کارجو")
     .Produces(StatusCodes.Status404NotFound)
     .Produces(StatusCodes.Status204NoContent)
     .Produces(StatusCodes.Status500InternalServerError)
     .RequireAuthorization(AuthPolicy.EmployeeOnly);
}
