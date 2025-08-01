using JobBoard.Domain.Entities;
using JobBoard.UnitTest.IdentityService.Shared;

namespace JobBoard.UnitTest.JobService;

public class UpdateJobTests : IDisposable
{
    private InMemoryDb _inMemoryDb;
    private JobFeatures.Services.JobService _unit;

    public UpdateJobTests()

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
    public async Task Returns_Success_When_JobExists(string title, string description, int salary)
    {
        // Given
        var job = Job.Create(title, description, Guid.NewGuid(), salary);
        _inMemoryDb.context.Add(job);
        _inMemoryDb.context.SaveChanges();
        // When
        var Response = await _unit.UpdateJob("new", "d", job.Id, CancellationToken.None);
        // Then
        Assert.True(Response.IsSuccess);
    }

    [Fact]
    public async Task Returns_Failure_When_JobDoesNotExists()
    {
        // Given
        // When
        var Response = await _unit.UpdateJob("new", "d", Guid.NewGuid(), CancellationToken.None);
        // Then
        Assert.False(Response.IsSuccess);
    }
}
