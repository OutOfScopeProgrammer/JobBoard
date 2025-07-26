using JobBoard.Shared.Utilities;

namespace JobBoard.FileFeatures;

public class ImageProcessor(IWebHostEnvironment env)
{
    private List<string> AllowedFormats { get; } = ["jpg", "jpeg", "png"];
    public async Task<Response<string>> SaveImage(IFormFile file)
    {
        ArgumentNullException.ThrowIfNull(file);
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedFormats.Contains(ext))
            return Response<string>.Failure(new Error(ErrorTypes.UnSupportedFormat, "image format is not supported."));
        var uploadPath = Path.Combine(env.WebRootPath, "cvImages");
        Directory.CreateDirectory(uploadPath);
        var storedName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(uploadPath, storedName);
        using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream);
        return Response<string>.Success(fullPath);
    }
}
