using JobBoard.Domain.Entities;
using JobBoard.Shared.Utilities;
using JobBoard.UnitTest.IdentityService.Shared;

namespace JobBoard.UnitTest.JobService;

public class GetJobsTests : IDisposable
{
    private JobFeatures.Services.JobService _unit;
    private InMemoryDb _inMemoryDb;
    public GetJobsTests()
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
    [InlineData("testJob", "testDescription", 2)]
    [InlineData("test Job title", "test description that is longer", 1234)]

    public async Task GetJobs_ReturnSuccessWithListOfJobs_When_JobsExists(string title, string description, int salary)
    {
        // Given
        var userId = _inMemoryDb.context.Users.Select(u => u.Id).FirstOrDefault();
        var job = Job.Create(title, description, userId, salary);
        _inMemoryDb.context.Add(job);
        _inMemoryDb.context.SaveChanges();
        // When
        var response = await _unit.GetJobs(CancellationToken.None);
        // Then
        Assert.True(response.IsSuccess);
        Assert.True(response.Data.FirstOrDefault()!.Title == job.Title);
        Assert.Equivalent(response.Data.FirstOrDefault(), job);

    }
    [Fact]
    public async Task GetJobs_ReturnFailure_WhenJobDoesNotExist()
    {
        // Given
        // When
        var response = await _unit.GetJobs(CancellationToken.None);
        // Then
        Assert.False(response.IsSuccess);
        Assert.Contains(ErrorMessages.NotFound, response.Errors);
    }
}
