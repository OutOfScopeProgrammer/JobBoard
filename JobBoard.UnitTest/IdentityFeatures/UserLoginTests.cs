
using JobBoard.Domain.Entities;
using JobBoard.IdentityFeatures.Services;
using JobBoard.Infrastructure.Auth;
using JobBoard.Shared.Utilities;
using JobBoard.UnitTest.IdentityFeatures.ClassFixtures;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Xunit.Sdk;

namespace JobBoard.UnitTest.IdentityFeatures;

public class UserLoginTests : IDisposable
{
    private InMemoryDb inMemoryDb;
    private PasswordHasher<User> hasher;
    private TokenProvider tokenProvider;
    private IdentityService unit;

    public UserLoginTests()
    {
        this.inMemoryDb = new InMemoryDb();
        var setting = new JwtSetting
        {
            Audience = "test",
            Issuer = "test",
            ExpirationInMinute = 1,
            Secret = "jhjfghjkljnhgyuikjhbjgftuyuhjknbhjgyuj",
        };
        hasher = new PasswordHasher<User>();
        var jwtSetting = Options.Create(setting);
        tokenProvider = new TokenProvider(jwtSetting);
        var context = inMemoryDb.context;
        unit = new IdentityService(context, tokenProvider, hasher);
    }

    public void Dispose()
    {
        inMemoryDb.Dispose();

    }

    [Theory]
    [InlineData("test1@gmail.com", "Correctpassword")]
    [InlineData("test2@gmail.com", "Correctpassword")]
    [InlineData("test3@gmail.com", "Correctpassword")]
    public async Task Login_Returns_SuccessWhenUserExistAndPasswordIsCorrect(string email, string password)
    {
        // ARRANGE
        var user = User.Create(email, "test");
        var role = inMemoryDb.context.Roles.FirstOrDefault(r => r.RoleName == "ADMIN");
        user.SetRole(role!);
        var hashedPassword = hasher.HashPassword(user, password);
        user.SetHashedPassword(hashedPassword);
        inMemoryDb.context.Users.Add(user);
        inMemoryDb.context.SaveChanges();
        // ACT
        var resposne = await unit.Login(email, password);
        // ASSERT
        Assert.True(resposne.IsSuccess);
        Assert.Equal(user.Name, resposne.Data.UserName);
    }


    [Theory]
    [InlineData("test1@gmail.com", "Correctpassword", "verywrongPassowrd1")]
    [InlineData("test2@gmail.com", "Correctpassword", "myWrongPassword@1212ww")]
    [InlineData("test3@gmail.com", "Correctpassword", "")]
    public async Task Login_Returns_FailureWhenUserExistAndPasswordIsNotCorrect(string email, string password, string wrongPassword)
    {
        // ARRANGE
        var user = User.Create(email, "test");
        var role = inMemoryDb.context.Roles.FirstOrDefault(r => r.RoleName == "ADMIN");
        user.SetRole(role!);
        var hashedPassword = hasher.HashPassword(user, wrongPassword);
        user.SetHashedPassword(hashedPassword);
        inMemoryDb.context.Users.Add(user);
        inMemoryDb.context.SaveChanges();
        // ACT
        var resposne = await unit.Login(email, password);
        // ASSERT
        Assert.False(resposne.IsSuccess);
        Assert.Equal(ErrorMessages.Unauthorized, resposne.Errors.FirstOrDefault());
    }
    [Theory]
    [InlineData("test1@gmail.com", "Correctpassword", "wrong@email.com")]
    [InlineData("test2@gmail.com", "Correctpassword", "very.wrong@gmail.com")]
    [InlineData("test3@gmail.com", "Correctpassword", "")]
    public async Task Login_Returns_FailureWhenUserDoesNotExistAndPasswordIsCorrect(string email, string password, string wrongEmail)
    {
        // ARRANGE
        var user = User.Create(email, "test");
        var role = inMemoryDb.context.Roles.FirstOrDefault(r => r.RoleName == "ADMIN");
        user.SetRole(role!);
        var hashedPassword = hasher.HashPassword(user, password);
        user.SetHashedPassword(hashedPassword);
        inMemoryDb.context.Users.Add(user);
        inMemoryDb.context.SaveChanges();
        // ACT
        var resposne = await unit.Login(wrongEmail, password);
        // ASSERT
        Assert.False(resposne.IsSuccess);
        Assert.Equal(ErrorMessages.Unauthorized, resposne.Errors.FirstOrDefault());
    }

}
