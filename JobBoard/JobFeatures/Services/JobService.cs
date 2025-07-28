using JobBoard.Domain.Entities;
using JobBoard.Shared.Persistence;
using JobBoard.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.JobFeatures.Services;

public class JobService(AppDbContext dbContext)
{
    public async Task<Response<List<Job>>> GetJobs(CancellationToken cancellationToken)
    {
        var jobs = await dbContext.Jobs.AsNoTracking()
            .ToListAsync(cancellationToken);

        if (jobs.FirstOrDefault() is null) return Response<List<Job>>.Failure(ErrorMessages.NotFound);
        return Response<List<Job>>.Success(jobs);
    }
    public async Task<Response<Job>> GetJobById(Guid jobId, CancellationToken cancellationToken)
    {
        var job = await dbContext.Jobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId);
        if (job is null)
            return Response<Job>.Failure(ErrorMessages.NotFound);
        return Response<Job>.Success(job);
    }
    public async Task<Response<Guid>> CreateJob(string title, string description, int salary, Guid userId, CancellationToken cancellationToken)
    {
        var job = Job.Create(title, description, userId, salary);
        dbContext.Jobs.Add(job);
        if (await dbContext.SaveChangesAsync(cancellationToken) <= 0)
            return Response<Guid>.Failure(ErrorMessages.Internal);
        return Response<Guid>.Success(job.Id);
    }

    public async Task<Response> UpdateJob(string? title, string? description, Guid jobId, CancellationToken cancellationToken)
    {
        var job = await dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);
        if (job is null) return Response.Failure(ErrorMessages.NotFound);
        job.UpdateJob(title, description);
        if (await dbContext.SaveChangesAsync(cancellationToken) <= 0)
            return Response.Failure(ErrorMessages.Internal);
        return Response.Success();

    }
}
