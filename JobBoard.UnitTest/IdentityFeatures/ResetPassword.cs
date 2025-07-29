using JobBoard.Domain.Entities;
using JobBoard.IdentityFeatures.Services;
using JobBoard.Infrastructure.Auth;
using JobBoard.Shared.Utilities;
using JobBoard.UnitTest.IdentityFeatures.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace JobBoard.UnitTest.IdentityFeatures;

public class ResetPassword : IDisposable
{
    private InMemoryDb inMemoryDb;
    private PasswordHasher<User> hasher;
    private TokenProvider tokenProvider;
    private IdentityService unit;
    public ResetPassword()
    {
        inMemoryDb = new InMemoryDb();
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
        unit = new IdentityService(inMemoryDb.context, tokenProvider, hasher);
    }

    public void Dispose()
    {
        inMemoryDb.context.Dispose();
    }


    [Theory]
    [InlineData("test@email.com", "OldPassword", "newPassword", "testGuy")]
    [InlineData("test1@email.com", "Old", "new", "2esdwe")]
    [InlineData("test2@email.com", "o2@ls", "n@kjw", "MyGuyNumber1")]
    [InlineData("test3@email.com", "", "newPassword", "Test124")]
    [InlineData("test4@email.com", "123old", "new1234", "Mlks235@")]

    public async Task ResetPassword_Returns_Success_When_UserEmailExistsAndOldPasswordIsCorrect
    (string email, string oldPassword, string newPassword, string userName)
    {
        // ARRANGE
        var user = User.Create(email, userName);
        var hashedPassword = hasher.HashPassword(user, oldPassword);
        user.SetHashedPassword(hashedPassword);
        var role = inMemoryDb.context.Roles.FirstOrDefault();
        user.SetRole(role!);
        inMemoryDb.context.Users.Add(user);
        inMemoryDb.context.SaveChanges();
        // ACT
        var respons = await unit.ResetPassword(email, oldPassword, newPassword);
        // ASSERT

        Assert.True(respons.IsSuccess);
        Assert.Equal(user.Name, respons.Data.UserName);
        Assert.Equal(role!.RoleName, respons.Data.Role);

    }


    [Theory]
    [InlineData("test@email.com", "OldPassword", "newPassword", "testGuy")]
    [InlineData("test1@email.com", "Old", "new", "2esdwe")]
    [InlineData("test2@email.com", "o2@ls", "n@kjw", "MyGuyNumber1")]
    [InlineData("test3@email.com", "", "newPassword", "Test124")]
    [InlineData("test4@email.com", "123old", "new1234", "Mlks235@")]

    public async Task ResetPassword_Returns_Failure_When_UserEmailExistsAndOldPasswordIsNotCorrect
    (string email, string wrongPassword, string newPassword, string userName)
    {
        // ARRANGE
        var user = User.Create(email, userName);
        var hashedPassword = hasher.HashPassword(user, "CorrectPassword");
        user.SetHashedPassword(hashedPassword);
        var role = inMemoryDb.context.Roles.FirstOrDefault();
        user.SetRole(role!);
        inMemoryDb.context.Users.Add(user);
        inMemoryDb.context.SaveChanges();
        // ACT
        var respons = await unit.ResetPassword(email, wrongPassword, newPassword);
        // ASSERT
        Assert.False(respons.IsSuccess);
        Assert.Equal(ErrorMessages.Unauthorized, respons.Errors.FirstOrDefault());

    }
    [Theory]
    [InlineData("test@email.com", "OldPassword", "newPassword", "testGuy")]
    [InlineData("test1@email.com", "Old", "new", "2esdwe")]
    [InlineData("test2@email.com", "o2@ls", "n@kjw", "MyGuyNumber1")]
    [InlineData("test3@email.com", "", "newPassword", "Test124")]
    [InlineData("test4@email.com", "123old", "new1234", "Mlks235@")]

    public async Task ResetPassword_Returns_Failure_When_UserEmailDoesNotExist
   (string email, string oldPassword, string newPassword, string userName)
    {
        // ARRANGE
        var user = User.Create("Correct@gmail.com", userName);
        var hashedPassword = hasher.HashPassword(user, oldPassword);
        user.SetHashedPassword(hashedPassword);
        var role = inMemoryDb.context.Roles.FirstOrDefault();
        user.SetRole(role!);
        inMemoryDb.context.Users.Add(user);
        inMemoryDb.context.SaveChanges();
        // ACT
        var respons = await unit.ResetPassword(email, oldPassword, newPassword);
        // ASSERT

        Assert.False(respons.IsSuccess);
        Assert.Equal(ErrorMessages.Unauthorized, respons.Errors.FirstOrDefault());

    }
}
