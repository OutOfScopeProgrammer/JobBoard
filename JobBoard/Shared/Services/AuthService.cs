using JobBoard.Shared.Auth;
using JobBoard.Shared.Domain.Entities;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Shared.Services;

public record AuthResponse(string UserName, string AccessToken);


public class AuthService(TokenProvider tokenProvider, IPasswordHasher<User> passwordHasher)
{

    public async Task<Response<AuthResponse>> CreateUser(string email, string name, string password)
    {
        var user = User.Create(email, name);
        var hashedPassword = passwordHasher.HashPassword(user, password);
        user.SetHashedPassword(hashedPassword);
        // give role,Save to db
        var token = tokenProvider.GenerateJwt(user);
        return Response<AuthResponse>.Success(new(user.Name, token));
    }

    public async Task<Response<AuthResponse>> Login(string email, string plainPassword)
    {
        // login functionality
        return Response<AuthResponse>.Success();
    }

}
