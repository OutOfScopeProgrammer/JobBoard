using JobBoard.Infrastructure.Auth;
using JobBoard.JobApplicationFeatures.Services;
using JobBoard.Shared.EndpointFilters;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.JobApplicationFeatures.GetApplication;

public class GetApplicationById : IEndpointMarker
{
    public void Register(IEndpointRouteBuilder app)
        => app.MapGroup("api")
        .MapGet("applications/{applicationId}", async ([FromRoute] Guid applicationId, JobApplicationService service,
         CancellationToken cancellationToken) =>
        {
            var response = await service.GetApplicationById(applicationId, cancellationToken);
            return response.IsSuccess ?
             Results.Ok(response.Data) :
             Results.NotFound(response.Errors);
        })
        .WithTags("Application")
        .WithDescription("دریافت رزومه با ایدی ")
        .WithSummary("Get applicaiton by id")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .AddEndpointFilter<LogginFilter>()
        .RequireAuthorization(AuthPolicy.EmployeeOnly);
}
