using JobBoard.JobFeatures.Mapper;
using JobBoard.JobFeatures.Services;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.JobFeatures.GetJob;

public class GetJobById : IEndpointMarker
{
    public RouteHandlerBuilder Register(IEndpointRouteBuilder app)
        => app.MapGroup("api")
        .MapGet("job/{id:guid}", async ([FromRoute] Guid id, JobService jobService, CancellationToken cancellationToken) =>
        {
            var response = await jobService.GetJobById(id, cancellationToken);
            return response.IsSuccess ?
            Results.Ok(JobMapper.MapToJobDto(response.Data)) :
            Results.NotFound(response.Errors);
        }).WithTags("Job")
        .WithDescription("دریافت شغل با ایدی")
        .WithSummary("Get a job by id")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}
