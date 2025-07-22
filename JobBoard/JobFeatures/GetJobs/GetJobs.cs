using JobBoard.JobFeatures.Services;
using JobBoard.Shared.EndpointFilters;
using JobBoard.Shared.Utilities;

namespace JobBoard.JobFeatures.GetJobs;

public class GetJobs : IEndpointMarker
{
    public void Register(IEndpointRouteBuilder app)
        => app.MapGroup("api")
        .MapGet("job", async (JobService jobService, CancellationToken cancellationToken) =>
        {
            var response = await jobService.GetJobs(cancellationToken);
            return response.IsSuccess ?
            Results.Ok(response.Data) :
            Results.NotFound(response.Errors);
        })
        .WithTags("Job")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .AddEndpointFilter<LogginFilter>();
}
