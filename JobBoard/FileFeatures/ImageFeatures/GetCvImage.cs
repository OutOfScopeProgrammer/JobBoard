using JobBoard.Shared.Utilities;

namespace JobBoard.FileFeatures.ImageFeatures;

public class GetCvImage : IEndpointMarker
{
    public void Register(IEndpointRouteBuilder app)
     => app.MapGroup("api/cv")
     .MapGet("images/{filename}", async (string fileName, IWebHostEnvironment env) =>
     {
         var imagePath = Path.Combine(env.WebRootPath, "userUpload", fileName);
         return Results.File(imagePath, "image/png");
     });
}
