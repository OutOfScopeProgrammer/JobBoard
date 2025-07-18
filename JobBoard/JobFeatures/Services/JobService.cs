using JobBoard.Shared.Domain.Entities;
using JobBoard.Shared.Persistence;
using JobBoard.Shared.Utilities;

namespace JobBoard.JobFeatures.Services;

public class JobService(AppDbContext dbContext)
{

    public async Task<Response<string>> CreateJob(string title, string description, Guid userId, CancellationToken cancellationToken)
    {
        var job = Job.Create(title, description, userId);
        dbContext.Jobs.Add(job);
        if (await dbContext.SaveChangesAsync(cancellationToken) <= 0)
            return Response<string>.Failure(new Error(ErrorTypes.Internal, "something went wrong"));
        return Response<string>.Success(job.Id.ToString());
    }
}
