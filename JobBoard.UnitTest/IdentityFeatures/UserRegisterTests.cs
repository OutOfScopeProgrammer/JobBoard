using JobBoard.Domain.Entities;
using JobBoard.IdentityFeatures.Services;
using JobBoard.Infrastructure.Auth;
using JobBoard.Shared.Persistence;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace JobBoard.UnitTest.IdentityFeatures;



public class UserRegisterTests : IAsyncLifetime
{
    private IdentityService unit;
    private AppDbContext context;
    private IPasswordHasher<User> hasher = Substitute.For<IPasswordHasher<User>>();
    private TokenProvider tokenProvider;

    public async Task InitializeAsync()
    {
        var setting = new JwtSetting
        {
            Audience = "test",
            Issuer = "test",
            ExpirationInMinute = 1,
            Secret = "jhjfghjkljnhgyuikjhbjgftuyuhjknbhjgyuj",
        };

        var roles = new List<Role> {
            new() { Id = Guid.NewGuid(), RoleName = "ADMIN" },
            new() { Id = Guid.NewGuid(), RoleName = "APPLICANT" },
            new() { Id = Guid.NewGuid(), RoleName = "EMPLOYEE" }
            };
        var options = new DbContextOptionsBuilder<AppDbContext>()
           .UseInMemoryDatabase(Guid.NewGuid().ToString())
           .Options;
        context = new AppDbContext(options);
        context.Roles.AddRange(roles);
        await context.SaveChangesAsync();
        hasher = new PasswordHasher<User>();
        var jwtSetting = Options.Create(setting);
        tokenProvider = new TokenProvider(jwtSetting);
        unit = new IdentityService(context, tokenProvider, hasher);
    }

    public async Task DisposeAsync()
    {
        await context.Database.EnsureDeletedAsync();
        await context.DisposeAsync();
    }


    [Theory]
    [InlineData("test1@gmail.com", "password1", "test1", "admin")]
    [InlineData("test2@gmail.com", "password1", "test2", "Applicant")]
    [InlineData("test3@gmail.com", "password1", "test3", "EMPLOYEE")]
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
}
