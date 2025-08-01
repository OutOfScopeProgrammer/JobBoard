using JobBoard.Domain.Entities;
using JobBoard.FileFeatures;
using JobBoard.Infrastructure.Auth;
using JobBoard.Shared.Persistence.Postgres;
using JobBoard.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.CvFeatures.Services;

public class CvService(AppDbContext dbContext, ImageProcessor imageProcessor)
{
    private readonly string SubFolder = "cvImages";

    public async Task<Response> CreateCv(string fullName, string? fullAddress,
     string city, int expectedSalary, IFormFile image, Guid userId)
    {
        var applicant = await dbContext.Users.AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId & u.Role.RoleName == ApplicationRoles.APPLICANT.ToString());
        if (applicant is null)
            return Response.Failure(ErrorMessages.NotFound);

        var hasCv = await dbContext.Cvs.SingleOrDefaultAsync(c => c.UserId == applicant.Id);
        if (hasCv is not null)
            return Response.Failure(ErrorMessages.Conflict);

        var cv = Cv.Create(fullName, fullAddress, city, expectedSalary, applicant.Id);
        if (image is not null)
        {
            var imageUrl = await imageProcessor
                .WithImage(image)
                .InSubFolder(SubFolder)
                .Save();

            if (!imageUrl.IsSuccess)
                return Response.Failure(imageUrl.Errors);

            cv.SetImage(imageUrl.Data!);
        }

        dbContext.Cvs.Add(cv);
        if (await dbContext.SaveChangesAsync() <= 0)
            return Response.Failure(ErrorMessages.Internal);

        return Response.Success();
    }

    public async Task<Response<Cv>> GetCvById(Guid cvId)
    {
        var cv = await dbContext.Cvs.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cvId);
        return cv is null ?
        Response<Cv>.Failure(ErrorMessages.NotFound) :
        Response<Cv>.Success(cv);
    }
}
