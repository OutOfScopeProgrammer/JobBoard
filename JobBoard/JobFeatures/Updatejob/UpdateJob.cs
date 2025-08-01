using System.Security.Claims;
using JobBoard.Infrastructure.Auth;
using JobBoard.JobFeatures.Services;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.JobFeatures.Updatejob;

public record UpdateJobDto(string? Title, string? Description, string JobId);
public class UpdateJob : IEndpointMarker
{
    public RouteHandlerBuilder Register(IEndpointRouteBuilder app)
     => app.MapGroup("api")
     .MapPut("job", async ([FromBody] UpdateJobDto dto, HttpContext context,
      JobService jobService, CancellationToken cancellationToken) =>
     {
         if (!Guid.TryParse(dto.JobId, out Guid jobId))
             return Results.BadRequest("Job id is not valid");
         var userId = AuthHelper.GetUserId(context);
         var response = await jobService.UpdateJob(dto.Title, dto.Description, jobId, userId, cancellationToken);
         if (!response.IsSuccess)
         {
             if (response.Errors.FirstOrDefault() == ErrorMessages.NotFound)
                 return Results.NotFound(response.Errors);

         }
         return Results.NoContent();

     }).WithTags("Job")
     .WithDescription("بروزرسانی شغل")
     .WithSummary("Update a job by id")
     .RequireAuthorization(AuthPolicy.EmployeeOnly)
     .Produces(StatusCodes.Status400BadRequest)
     .Produces(StatusCodes.Status204NoContent)
     .Produces(StatusCodes.Status500InternalServerError)
     .Produces(StatusCodes.Status404NotFound);
}
