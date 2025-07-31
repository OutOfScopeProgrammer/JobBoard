
using JobBoard.Domain.Entities;
using JobBoard.Infrastructure.Auth;
using JobBoard.Shared.Persistence;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.IdentityFeatures.Services;

public record AuthResponse(string UserName, string AccessToken, string Role);


public class IdentityService(AppDbContext dbContext, TokenProvider tokenProvider, IPasswordHasher<User> passwordHasher)
{

    public async Task<Response<AuthResponse>> CreateUser(string email, string name, string password, string roleName)
    {
        if (await dbContext.Users.AnyAsync(u => u.Email == email))
            return Response<AuthResponse>.Failure(ErrorMessages.Conflict);
        var user = User.Create(email, name);
        var hashedPassword = passwordHasher.HashPassword(user, password);
        user.SetHashedPassword(hashedPassword);
        var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName.ToUpper());
        if (role is null)
            return Response<AuthResponse>.Failure(ErrorMessages.InvalidRole);

        user.SetRole(role);
        dbContext.Add(user);

        await dbContext.SaveChangesAsync();
        var token = tokenProvider.GenerateJwt(user);
        return Response<AuthResponse>.Success(new(user.Name, token, user.Role.RoleName));
    }

    public async Task<Response<AuthResponse>> Login(string email, string password)
    {
        var user = await dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
            return Response<AuthResponse>.Failure(ErrorMessages.Unauthorized);
        var checkPassword = passwordHasher.VerifyHashedPassword(user, user.HashedPassword, password);
        if (checkPassword == PasswordVerificationResult.Failed)
            return Response<AuthResponse>.Failure(ErrorMessages.Unauthorized);

        var token = tokenProvider.GenerateJwt(user);
        return Response<AuthResponse>.Success(new(user.Name, token, user.Role.RoleName));
    }


    public async Task<Response<AuthResponse>> ResetPassword(string email, string oldPassword, string newPassword)
    {
        var user = await dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
            return Response<AuthResponse>.Failure(ErrorMessages.Unauthorized);

        var checkOldPassword = passwordHasher.VerifyHashedPassword(user, user.HashedPassword, oldPassword);
        if (checkOldPassword == PasswordVerificationResult.Failed)
            return Response<AuthResponse>.Failure(ErrorMessages.Unauthorized);

        var newHashedPassword = passwordHasher.HashPassword(user, newPassword);
        user.SetHashedPassword(newHashedPassword);
        await dbContext.SaveChangesAsync();
        var token = tokenProvider.GenerateJwt(user);
        return Response<AuthResponse>.Success(new(user.Name, token, user.Role.RoleName));
    }
}
