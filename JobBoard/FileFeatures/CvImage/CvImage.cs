using JobBoard.Shared.Utilities;

namespace JobBoard.FileFeatures.CvImage;

public class CvImage : IEndpointMarker
{
    public RouteHandlerBuilder Register(IEndpointRouteBuilder app)
        => app.MapGroup("api/images")
        .MapGet("cv/{imageName}", async (string imageName, IWebHostEnvironment env) =>
        {
            var path = Path.Combine(env.WebRootPath, "cvImages", imageName);
            var imageExtension = Path.GetExtension(path);
            _ = imageExtension.Replace('.', '/');
            var image = await File.ReadAllBytesAsync(path);
            return Results.File(image, $"image{imageExtension}");
        });
}
