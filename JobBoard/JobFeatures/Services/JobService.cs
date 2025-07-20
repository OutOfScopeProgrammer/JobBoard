using JobBoard.Shared.Domain.Entities;
using JobBoard.Shared.Persistence;
using JobBoard.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.JobFeatures.Services;

public class JobService(AppDbContext dbContext)
{
    public async Task<Response<List<Job>>> GetJobs(CancellationToken cancellationToken)
    {
        var jobs = await dbContext.Jobs.ToListAsync(cancellationToken);
        if (jobs is null) return Response<List<Job>>.Failure(new Error(ErrorTypes.NotFound, "jobs not found"));
        return Response<List<Job>>.Success(jobs);
    }
    public async Task<Response<Job>> GetJob(Guid jobId, CancellationToken cancellationToken)
    {
        var job = await dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);
        if (job is null)
            return Response<Job>.Failure(new Error(ErrorTypes.NotFound, "Job not found"));
        return Response<Job>.Success(job);
    }
    public async Task<Response<Guid>> CreateJob(string title, string description, Guid userId, CancellationToken cancellationToken)
    {
        var job = Job.Create(title, description, userId);
        dbContext.Jobs.Add(job);
        if (await dbContext.SaveChangesAsync(cancellationToken) <= 0)
            return Response<Guid>.Failure(new Error(ErrorTypes.Internal, "Something went wrong"));
        return Response<Guid>.Success(job.Id);
    }

    public async Task<Response<bool>> UpdateJob(string? title, string? description, Guid jobId, CancellationToken cancellationToken)
    {
        var job = await dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);
        if (job is null) return Response<bool>.Failure(new Error(ErrorTypes.NotFound, "Job not found"));
        job.updateJob(title, description);
        if (await dbContext.SaveChangesAsync(cancellationToken) <= 0)
            return Response<bool>.Failure(new Error(ErrorTypes.Internal, "Something went wrong"));
        return Response<bool>.Success();

    }
}
