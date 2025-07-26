using JobBoard.CvFeatures.Mapper;
using JobBoard.CvFeatures.Services;
using JobBoard.Domain.Entities;
using JobBoard.Shared.Utilities;

namespace JobBoard.CvFeatures.GetCvById;

public class GetCvById : IEndpointMarker
{
    public RouteHandlerBuilder Register(IEndpointRouteBuilder app)
        => app.MapGroup("api")
        .MapGet("cv/{userId:guid}", async (Guid userId, CvService cvService, HttpContext context) =>
        {
            var response = await cvService.GetCvById(userId);
            if (!response.IsSuccess)
                return Results.NotFound(response.Errors);

            CvMapper.MapToCvDto(response.Data, context.Request.Scheme, context.Request.Host.ToString());
            return Results.Ok(response);
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();
}
