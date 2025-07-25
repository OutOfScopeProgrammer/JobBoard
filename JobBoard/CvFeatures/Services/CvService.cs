using System.Threading.Tasks;
using JobBoard.Domain.Entities;
using JobBoard.Infrastructure.Auth;
using JobBoard.Shared.Persistence;
using JobBoard.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.CvFeatures.Services;

public class CvService(AppDbContext dbContext, IWebHostEnvironment env)
{

    public async Task<Response<bool>> CreateCv(string fullName, string? fullAddress,
     string city, int expectedSalary, IFormFile image, Guid userId)
    {
        var applicant = await dbContext.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId & u.Role.RoleName == ApplicationRoles.APPLICANT.ToString());
        if (applicant is null)
            return Response<bool>.Failure(new Error(ErrorTypes.NotFound, "user not found"));

        var imageUrl = await SaveFile(image);
        var cv = Cv.Create(fullName, fullAddress, city, expectedSalary, imageUrl, applicant.Id);
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


    private async Task<string> SaveFile(IFormFile file)
    {
        if (file is null) return "null";
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var uploadPath = Path.Combine(env.WebRootPath, "userUpload");
        Directory.CreateDirectory(uploadPath);
        var storedName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(uploadPath, storedName);
        using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream);
        return fullPath;
    }

}
