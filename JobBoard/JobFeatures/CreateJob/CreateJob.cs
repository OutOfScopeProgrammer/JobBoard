using JobBoard.Infrastructure.Auth;
using JobBoard.JobFeatures.Services;
using JobBoard.Shared.EndpointFilters;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.JobFeatures.CreateJob;

public record CreateJobDto(string Title, string Description, int Salary);

public class CreateJob : IEndpointMarker
{
    public RouteHandlerBuilder Register(IEndpointRouteBuilder app)
        => app.MapGroup("api")
        .MapPost("job", async ([FromBody] CreateJobDto dto,
        HttpContext context, JobService jobService, CancellationToken cancellationToken) =>
        {
            var userId = AuthHelper.GetUserId(context);
            var response = await jobService.CreateJob(dto.Title, dto.Description, dto.Salary, userId, cancellationToken);
            if (!response.IsSuccess)
                return Results.InternalServerError();
            return Results.Created();
        })
        .WithTags("Job")
        .WithDescription("ایجاد شغل")
        .WithSummary("Create a job")
        .AddEndpointFilter<ValidationFilter<CreateJobDto>>()
        .RequireAuthorization(AuthPolicy.EmployeeOnly)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status500InternalServerError);
}
