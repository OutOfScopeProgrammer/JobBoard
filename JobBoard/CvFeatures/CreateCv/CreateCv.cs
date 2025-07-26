using JobBoard.CvFeatures.Services;
using JobBoard.Infrastructure.Auth;
using JobBoard.Shared.EndpointFilters;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.CvFeatures.CreateCv;

public record CreateCvDto(string FullName, string? FullAddress, string City, int ExpectedSalary, IFormFile Image);
public class CreateCv : IEndpointMarker
{
    public RouteHandlerBuilder Register(IEndpointRouteBuilder app)
        => app.MapGroup("api")
        .MapPost("cv", async ([FromForm] CreateCvDto dto, CvService cvService, HttpContext context) =>
        {
            var contentType = context.Request.ContentType;
            if (!contentType!.Contains("multipart/form-data"))
                return Results.BadRequest("content-type must be multipart/form-data");

            var userId = AuthHelper.GetUserId(context);
            var response = await cvService.CreateCv(dto.FullName, dto.FullAddress, dto.City, dto.ExpectedSalary, dto.Image, userId);
            return response.IsSuccess ?
            Results.Created() :
            Results.BadRequest(response.Errors);

        })
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .AddEndpointFilter<ValidationFilter<CreateCvDto>>()
        .RequireAuthorization(AuthPolicy.ApplicantOnly);
}
