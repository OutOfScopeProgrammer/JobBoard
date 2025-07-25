using JobBoard.Infrastructure.Auth;
using JobBoard.JobApplicationFeatures.Services;
using JobBoard.Shared.EndpointFilters;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.JobApplicationFeatures.GetApplications;

public class GetApplicationsByJobId : IEndpointMarker
{
    public RouteHandlerBuilder Register(IEndpointRouteBuilder app)
        => app.MapGroup("api")
        .MapGroup("job/{jobId}")
        .MapGet("applications", async ([FromRoute] Guid jobId,
         JobApplicationService service, CancellationToken cancellationToken) =>
        {
            var response = await service.GetApplicationsByJobId(jobId, cancellationToken);

            return response.IsSuccess ?
            Results.Ok(response.Data) :
            Results.NotFound(response.Errors);
        })
        .WithTags("Application")
        .WithDescription("دریافت رزومه های ارسالی برای اگهی ")
        .WithSummary("Get applications of a job by job id")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization(AuthPolicy.EmployeeOnly);
}
