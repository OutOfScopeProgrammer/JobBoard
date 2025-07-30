using JobBoard.Shared.Utilities;

namespace JobBoard.FileFeatures;

public class ImageProcessor(IWebHostEnvironment env)
{
    private List<string> AllowedFormats { get; } = [".jpg", ".jpeg", ".png"];
    private IFormFile? _image;
    private string _subFolderName = string.Empty;

    public ImageProcessor WithImage(IFormFile image)
    {
        ArgumentNullException.ThrowIfNull(image);

        _image = image;
        return this;
    }

    public ImageProcessor InSubFolder(string subFolderName)
    {
        _subFolderName = subFolderName;
        return this;
    }


    public async Task<Response<string>> Save()
    {
        ArgumentNullException.ThrowIfNull(_image);

        var ext = Path.GetExtension(_image.FileName).ToLowerInvariant();
        if (!AllowedFormats.Contains(ext))
            return Response<string>.Failure(ErrorMessages.UnsupportedFormat);

        var uploadPath = Path.Combine(env.WebRootPath, _subFolderName);
        Directory.CreateDirectory(uploadPath);
        var storedName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(uploadPath, storedName);
        using var fileStream = File.Create(fullPath);
        await _image.CopyToAsync(fileStream);
        return Response<string>.Success(fullPath);
    }

}
