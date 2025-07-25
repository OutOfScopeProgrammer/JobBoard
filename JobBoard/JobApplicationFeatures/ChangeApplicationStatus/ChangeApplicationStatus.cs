using JobBoard.Domain.Enums;
using JobBoard.Infrastructure.Auth;
using JobBoard.JobApplicationFeatures.Services;
using JobBoard.Shared.EndpointFilters;
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
             var apiResponse = response.Errors.FirstOrDefault()?.ErrorType switch
             {
                 ErrorTypes.NotFound => Results.NotFound(response.Errors),
                 _ => Results.InternalServerError(response.Errors)
             };
             return apiResponse;
         }
         return Results.NoContent();
     })
     .WithTags("Application")
     .WithSummary("Change application status")
     .WithDescription("تغییر وضعیت رزومه کارجو")
     .RequireAuthorization(AuthPolicy.EmployeeOnly)
     .AddEndpointFilter<LogFilter>();
}
