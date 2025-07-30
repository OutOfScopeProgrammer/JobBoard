using System.Threading.Tasks;
using JobBoard.Domain.Entities;
using JobBoard.UnitTest.IdentityService.Shared;

namespace JobBoard.UnitTest.JobService;

public class CreateJobTests : IDisposable
{
    private InMemoryDb _inMemoryDb;
    private JobFeatures.Services.JobService _unit;

    public CreateJobTests()

    {
        _inMemoryDb = new InMemoryDb();
        _unit = new JobFeatures.Services.JobService(_inMemoryDb.context);
        var user = User.Create("test@gmail.com", "test");
        _inMemoryDb.context.Users.Add(user);
        _inMemoryDb.context.SaveChanges();
    }

    public void Dispose()
    {
        _inMemoryDb.Dispose();
    }
    [Theory]
    [InlineData("title", "description", 5)]
    public async Task Returns_Success_When_UserExist(string title, string description, int salary)
    {
        // Given
        var user = User.Create("test@gmail.com", "test");
        _inMemoryDb.context.Users.Add(user);
        _inMemoryDb.context.SaveChanges();
        // When
        var response = await _unit.CreateJob(title, description, salary, user.Id, CancellationToken.None);
        // Then
        Assert.True(response.IsSuccess);
        Assert.IsType<Guid>(response.Data);
    }
}
