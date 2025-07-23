using System.Security.Claims;
using JobBoard.Infrastructure.Auth;
using JobBoard.JobApplicationFeatures.Services;
using JobBoard.Shared.EndpointFilters;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.UserFeature.ApplyToJob;

public record ApplyToJobDto(string Description, Guid JobId);
public class ApplyToJob : IEndpointMarker
{
    public void Register(IEndpointRouteBuilder app)
     => app.MapGroup("api")
     .MapGroup("job")
     .MapPost("application", async ([FromBody] ApplyToJobDto dto,
      JobApplicationService service, CancellationToken cancellationToken, HttpContext context) =>
    {
        var token = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid.TryParse(token, out Guid applicantId);
        var response = await service.ApplyToJobById(dto.JobId, applicantId, dto.Description, cancellationToken);
        if (!response.IsSuccess)
        {
            var apiResponse = response.Errors.FirstOrDefault()?.ErrorType switch
            {
                ErrorTypes.NotFound => Results.NotFound(response.Errors),
                _ => Results.InternalServerError(response.Errors),
            };
            return apiResponse;
        }
        return Results.Created();
    })
    .WithTags("Application")
    .WithDescription("ارسال رزومه برای آگهی ")
    .WithSummary("Apply to job")
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError)
    .AddEndpointFilter<LogFilter>()
    .RequireAuthorization(AuthPolicy.ApplicantOnly);
}
