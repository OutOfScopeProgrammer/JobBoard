using JobBoard.JobFeatures.Services;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.JobFeatures.Updatejob;

public record UpdateJobDto(string? Title, string? Description, string JobId);
public class UpdateJob : IEndpointMarker
{
    public void Register(IEndpointRouteBuilder app)
     => app.MapGroup("api")
     .MapPut("job", async ([FromBody] UpdateJobDto dto, JobService jobService, CancellationToken cancellationToken) =>
     {
         if (Guid.TryParse(dto.JobId, out var jobId))
             return Results.BadRequest("Job id is not valid");
         var response = await jobService.UpdateJob(dto.Title, dto.Description, jobId, cancellationToken);
         if (!response.IsSuccess)
         {
             var apiResponse = response.Errors.FirstOrDefault()?.ErrorType switch
             {
                 ErrorTypes.NotFound => Results.NotFound(response.Errors),
                 _ => Results.NotFound(response.Errors),
             };
             return apiResponse;
         }
         return Results.NoContent();

     }).WithTags("Job")
     .RequireAuthorization("Employee")
     .Produces(StatusCodes.Status400BadRequest)
     .Produces(StatusCodes.Status204NoContent)
     .Produces(StatusCodes.Status500InternalServerError)
     .Produces(StatusCodes.Status404NotFound);
}
