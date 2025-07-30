using JobBoard.Domain.Entities;
using JobBoard.Shared.Utilities;
using JobBoard.UnitTest.IdentityService.Shared;

namespace JobBoard.UnitTest.JobService;

public class GetJobByIdTests : IDisposable
{
    private JobFeatures.Services.JobService _unit;
    private Guid _jobId;

    private InMemoryDb _inMemoryDb;
    public GetJobByIdTests()
    {
        _inMemoryDb = new InMemoryDb();
        _unit = new JobFeatures.Services.JobService(_inMemoryDb.context);
        var user = User.Create("test@gmail.com", "test");
        _inMemoryDb.context.Add(user);
        var job = Job.Create("title", "desc", user.Id, 2);
        _inMemoryDb.context.Add(job);
        _inMemoryDb.context.SaveChanges();
        _jobId = job.Id;
    }

    public void Dispose()
    {
        _inMemoryDb.Dispose();
    }

    [Fact]
    public async Task Returns_job_When_JobDoesExist()
    {
        // Given
        // When
        var response = await _unit.GetJobById(_jobId, CancellationToken.None);
        // Then
        Assert.True(response.IsSuccess);
        Assert.IsType<Job>(response.Data);
    }

    [Fact]
    public async Task Returns_Failure_When_JobDoesNotExist()
    {
        // Given
        // When
        var response = await _unit.GetJobById(Guid.NewGuid(), CancellationToken.None);
        // Then
        Assert.False(response.IsSuccess);
        Assert.Equal(ErrorMessages.NotFound, response.Errors.FirstOrDefault());
    }
}
