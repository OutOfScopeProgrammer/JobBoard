using JobBoard.Domain.Entities;
using JobBoard.IdentityFeatures.Services;
using JobBoard.Infrastructure.Auth;
using JobBoard.Shared.Utilities;
using JobBoard.UnitTest.IdentityFeatures.ClassFixtures;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace JobBoard.UnitTest.IdentityFeatures;



public class UserRegisterTests : IDisposable
{
    private IdentityService unit;
    private IPasswordHasher<User> hasher = Substitute.For<IPasswordHasher<User>>();
    private TokenProvider tokenProvider;
    private readonly InMemoryDb inMemoryDb;

    public UserRegisterTests()
    {
        var setting = new JwtSetting
        {
            Audience = "test",
            Issuer = "test",
            ExpirationInMinute = 1,
            Secret = "jhjfghjkljnhgyuikjhbjgftuyuhjknbhjgyuj",
        };
        this.inMemoryDb = new InMemoryDb();
        hasher = new PasswordHasher<User>();
        var jwtSetting = Options.Create(setting);
        tokenProvider = new TokenProvider(jwtSetting);
        var context = inMemoryDb.context;
        unit = new IdentityService(context, tokenProvider, hasher);
    }


    [Theory]
    [InlineData("test3@gmail.com", "password1", "test3", "admin")]
    [InlineData("test1@gmail.com", "password1", "test1", "admin")]
    [InlineData("test2@gmail.com", "password1", "test2", "Applicant")]
    public async Task CreateUser_ReturnsAuthResponseSuccess_When_RoleIsCorrect_EmailIsUnique
    (string email, string password, string name, string roleName)
    {
        // ARRANGE
        // ACT
        var response = await unit.CreateUser(email, name, password, roleName);
        // ASSERT
        Assert.True(response.IsSuccess);
        Assert.Equal(name, response.Data.UserName);
        Assert.Equal(roleName.ToUpper(), response.Data.Role);
    }

    [Theory]
    [InlineData("test1@gmail.com", "password1", "test1", "wrongRole")]
    [InlineData("test2@gmail.com", "password1", "test2", "wrongRole")]
    [InlineData("test3@gmail.com", "password1", "test3", "WRONGROLE")]
    public async Task CreateUser_ReturnsAuthResponseFailure_When_RoleIsNotCorrect_EmailIsUnique
    (string email, string password, string name, string roleName)
    {
        // ARRANGE

        // ACT
        var response = await unit.CreateUser(email, name, password, roleName);

        // ASSERT
        Assert.False(response.IsSuccess);
        Assert.Equal(ErrorMessages.InvalidRole.ToString(), response.Errors.FirstOrDefault());
    }

    [Theory]
    [InlineData("test1@gmail.com", "password1", "test1", "Admin")]
    [InlineData("test1@gmail.com", "password1", "test2", "Admin")]
    [InlineData("test1@gmail.com", "password1", "test3", "Admin")]
    public async Task CreateUser_ReturnsAuthResponseFailure_When_EmailIsNotUnique
    (string email, string password, string name, string roleName)
    {
        // ARRANGE
        var firstUser = await unit.CreateUser("test1@gmail.com", "s", "password", "Applicant");
        // ACT
        var response = await unit.CreateUser(email, name, password, roleName);
        // ASSERT
        Assert.False(response.IsSuccess);
        Assert.Equal(ErrorMessages.Conflict.ToString(), response.Errors.FirstOrDefault());
    }

    public void Dispose()
    {
        inMemoryDb.Dispose();
    }
}
