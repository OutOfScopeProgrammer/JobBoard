
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
            return Response<AuthResponse>.Failure(new Error(ErrorTypes.Conflict, "email is already used"));
        var user = User.Create(email, name);
        var hashedPassword = passwordHasher.HashPassword(user, password);
        user.SetHashedPassword(hashedPassword);
        var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName.ToUpper());
        if (role is null)
            return Response<AuthResponse>.Failure(new Error(ErrorTypes.InvalidRole, "role does not exist"));

        user.SetRole(role);
        dbContext.Add(user);

        if (await dbContext.SaveChangesAsync() <= 0)
            return Response<AuthResponse>.Failure(new Error(ErrorTypes.Internal, "something went wrong when registering the user"));

        var token = tokenProvider.GenerateJwt(user);
        return Response<AuthResponse>.Success(new(user.Name, token, user.Role.RoleName));
    }

    public async Task<Response<AuthResponse>> Login(string email, string password)
    {
        var user = await dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
            return Response<AuthResponse>.Failure(new Error(ErrorTypes.Unauthorized, "user or password is wrong"));
        var checkPassword = passwordHasher.VerifyHashedPassword(user, user.HashedPassword, password);
        if (checkPassword == PasswordVerificationResult.Failed)
            return Response<AuthResponse>.Failure(new Error(ErrorTypes.Unauthorized, "user or password is wrong"));

        var token = tokenProvider.GenerateJwt(user);
        return Response<AuthResponse>.Success(new(user.Name, token, user.Role.RoleName));
    }


    public async Task<Response<AuthResponse>> ResetPassword(string email, string oldPassword, string newPassword)
    {
        var user = await dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
            return Response<AuthResponse>.Failure(new Error(ErrorTypes.Unauthorized, "user or password is wrong"));

        var checkOldPassword = passwordHasher.VerifyHashedPassword(user, user.HashedPassword, oldPassword);
        if (checkOldPassword == PasswordVerificationResult.Failed)
            return Response<AuthResponse>.Failure(new Error(ErrorTypes.Unauthorized, "user or password is wrong"));

        var newHashedPassword = passwordHasher.HashPassword(user, newPassword);
        user.SetHashedPassword(newHashedPassword);

        if (await dbContext.SaveChangesAsync() <= 0)
            return Response<AuthResponse>.Failure(new Error(ErrorTypes.Internal, "Something went wrong when registering the user"));

        var token = tokenProvider.GenerateJwt(user);
        return Response<AuthResponse>.Success(new(user.Name, token, user.Role.RoleName));
    }
}
