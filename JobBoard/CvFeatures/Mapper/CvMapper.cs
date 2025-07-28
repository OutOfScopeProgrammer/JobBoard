using JobBoard.Domain.Entities;

namespace JobBoard.CvFeatures.Mapper;

public record CvDto(Guid Id, string Fullname, string FullAddress, string City, int ExpectedSalary, string? ImageUrl);
public static class CvMapper
{
    public static CvDto MapToCvDto(Cv cv, string scheme, string host)
    {
        ArgumentNullException.ThrowIfNull(cv);
        CvDto cvDto;
        if (cv.ImageUrl is not null)
        {
            var imageName = Path.GetFileName(cv.ImageUrl);
            var baseUrl = $"{scheme}://{host}";
            var imageUrl = $"{baseUrl}/api/images/cv/{imageName}";
            cvDto = new(cv.Id, cv.FullName, cv.FullAddress, cv.City, cv.ExpectedSalary, imageUrl);
        }
        else
        {
            cvDto = new(cv.Id, cv.FullName, cv.FullAddress, cv.City, cv.ExpectedSalary, null);
        }
        return cvDto;
    }
}
