using JobBoard.Shared.Domain.Entities;
using JobBoard.Shared.Domain.Enums;
using JobBoard.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.ApplicationFeature.services;

public class ApplicationService(AppDbContext dbContext)
{

    public async Task<bool> ApplyToJobById(string description, Guid jobId, Guid applicantId, CancellationToken cancellationToken)
    {
        var job = await dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);
        if (job is null) return false;
        var application = Application.Create(description, jobId, applicantId, Status.InProgress);
        job.ApplyToJob(application);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
