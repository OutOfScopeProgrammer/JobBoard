using JobBoard.Domain.Entities;
using JobBoard.Domain.Enums;
using JobBoard.Shared.Persistence.Postgres;
using JobBoard.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.JobApplicationFeatures.Services;

public class JobApplicationService(AppDbContext dbContext)
{
    public async Task<Response<Guid>> ApplyToJobById(Guid jobId, Guid applicantId,
     string description, CancellationToken cancellationToken)
    {
        var job = await dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
        if (job is null)
            return Response<Guid>.Failure(ErrorMessages.NotFound);
        var application = Application.Create(description, job.Id, applicantId, Status.Submitted);
        dbContext.Applications.Add(application);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<Guid>.Success(application.Id);
    }


    public async Task<Response<Application>> GetApplicationById(Guid applicationId, CancellationToken cancellationToken)
    {
        var application = await dbContext.Applications
        .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);
        return application is null ?
        Response<Application>.Failure(ErrorMessages.NotFound) :
        Response<Application>.Success(application);
    }

    public async Task<Response<List<Application>>> GetApplicationsByJobId(Guid jobId, CancellationToken cancellationToken)
    {
        var applications = await dbContext.Applications
        .Where(a => a.JobId == jobId).ToListAsync(cancellationToken);
        return applications is null ?
        Response<List<Application>>.Failure(ErrorMessages.NotFound) :
        Response<List<Application>>.Success(applications);
    }

    public async Task<Response> ChangeApplicationStatus(Guid applicationId, Status status, CancellationToken cancellationToken)
    {
        var application = await dbContext.Applications.FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);
        if (application is null)
            return Response.Failure(ErrorMessages.NotFound);
        application.ChangeStatus(status);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Response.Success();
    }

}
