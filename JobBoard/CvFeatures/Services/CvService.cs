using JobBoard.Domain.Entities;
using JobBoard.FileFeatures;
using JobBoard.Infrastructure.Auth;
using JobBoard.Shared.Persistence;
using JobBoard.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.CvFeatures.Services;

public class CvService(AppDbContext dbContext, ImageProcessor imageProcessor)
{

    public async Task<Response<bool>> CreateCv(string fullName, string? fullAddress,
     string city, int expectedSalary, IFormFile image, Guid userId)
    {
        var applicant = await dbContext.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId & u.Role.RoleName == ApplicationRoles.APPLICANT.ToString());
        if (applicant is null)
            return Response<bool>.Failure(new Error(ErrorTypes.NotFound, "user not found"));
        var cv = Cv.Create(fullName, fullAddress, city, expectedSalary, applicant.Id);
        if (image is not null)
        {
            var imageUrl = await imageProcessor.SaveImage(image);
            if (!imageUrl.IsSuccess)
                return Response<bool>.Failure(imageUrl.Errors);

            cv.SetImage(imageUrl.Data!);
        }

        dbContext.Cvs.Add(cv);
        if (await dbContext.SaveChangesAsync() <= 0)
            return Response<bool>.Failure(new Error(ErrorTypes.Internal, "internalserverError"));

        return Response<bool>.Success();
    }

    public async Task<Response<Cv>> GetCvById(Guid userId)
    {
        var cv = await dbContext.Cvs.FirstOrDefaultAsync(c => c.UserId == userId);
        return cv is null ?
        Response<Cv>.Failure(new Error(ErrorTypes.NotFound, "not found.")) :
        Response<Cv>.Success(cv);
    }
}
