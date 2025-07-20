using JobBoard.JobFeatures.Services;
using JobBoard.Shared.Auth;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.JobFeatures.CreateJob;

public record CreateJobDto(string Title, string Description);

public class CreateJob : IEndpointMarker
{
    public void Register(IEndpointRouteBuilder app)
        => app.MapGroup("api")
        .MapPost("job", async ([FromBody] CreateJobDto dto,
        HttpContext context, JobService jobService, CancellationToken cancellationToken) =>
        {
            var userId = AuthHelper.GetUserId(context);
            var response = await jobService.CreateJob(dto.Title, dto.Description, userId, cancellationToken);
            if (!response.IsSuccess)
                return Results.InternalServerError();
            return Results.Created();
        })
        .WithTags("Job")
        .RequireAuthorization("EmployeeOnly")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status500InternalServerError);
}
