using JobBoard.JobFeatures.Mapper;
using JobBoard.JobFeatures.Services;
using JobBoard.Shared.Utilities;

namespace JobBoard.JobFeatures.GetJobs;

public class GetJobs : IEndpointMarker
{
    public RouteHandlerBuilder Register(IEndpointRouteBuilder app)
        => app.MapGroup("api")
        .MapGet("job", async (JobService jobService, CancellationToken cancellationToken) =>
        {
            var response = await jobService.GetJobs(cancellationToken);
            return response.IsSuccess ?
            Results.Ok(JobMapper.MapToJobDto(response.Data)) :
            Results.NotFound(response.Errors);
        })
        .WithTags("Job")
        .WithDescription("دریافت شغل ها")
        .WithSummary("Get all jobs")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}
