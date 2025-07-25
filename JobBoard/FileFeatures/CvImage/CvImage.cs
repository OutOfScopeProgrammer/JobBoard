using JobBoard.Shared.Utilities;

namespace JobBoard.FileFeatures.CvImage;

public class CvImage : IEndpointMarker
{
    public RouteHandlerBuilder Register(IEndpointRouteBuilder app)
        => app.MapGroup("api/images")
        .MapGet("cv/{imageName}", async (string imageName, IWebHostEnvironment env) =>
        {
            var path = Path.Combine(env.WebRootPath, "cvImages", imageName);
            var image = File.ReadAllBytes(path);
            return Results.File(image, "image/png");
        });
}
