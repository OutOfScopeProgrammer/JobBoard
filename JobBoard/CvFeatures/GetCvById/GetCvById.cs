using JobBoard.CvFeatures.Services;
using JobBoard.Shared.Utilities;

namespace JobBoard.CvFeatures.GetCvById;

public class GetCvById : IEndpointMarker
{
    public RouteHandlerBuilder Register(IEndpointRouteBuilder app)
        => app.MapGroup("api")
        .MapGet("cv/{userId:guid}", async (Guid userId, CvService cvService, HttpContext context) =>
        {
            var response = await cvService.GetCvById(userId);
            var imageName = Path.GetFileName(response.Data.ImageUrl);
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
            var imageUrl = $"{baseUrl}/api/images/cv/{imageName}";
            response.Data.ImageUrl = imageUrl;
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response.Errors);
        });
}
