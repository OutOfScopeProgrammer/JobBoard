using JobBoard.Domain.Entities;
using JobBoard.Domain.Enums;
using JobBoard.Shared.Persistence;
using JobBoard.Shared.Utilities;
using Microsoft.EntityFrameworkCore;
using Npgsql.Internal;

namespace JobBoard.JobApplicationFeatures.Services;

public class JobApplicationService(AppDbContext dbContext)
{
    public async Task<Response<Guid>> ApplyToJobById(Guid jobId, Guid applicantId,
     string description, CancellationToken cancellationToken)
    {
        var job = await dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
        if (job is null)
            return Response<Guid>.Failure(new Error(ErrorTypes.NotFound, "Job is not found"));
        var application = Application.Create(description, job.Id, applicantId, Status.Submitted);
        dbContext.Applications.Add(application);
        if (await dbContext.SaveChangesAsync(cancellationToken) <= 0)
            return Response<Guid>.Failure(new Error(ErrorTypes.Internal, "internal server error"));

        return Response<Guid>.Success(application.Id);
    }


    public async Task<Response<Application>> GetApplicationById(Guid applicationId, CancellationToken cancellationToken)
    {
        var application = await dbContext.Applications
        .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);
        return application is null ?
        Response<Application>.Failure(new Error(ErrorTypes.NotFound, "Application not Found")) :
        Response<Application>.Success(application);
    }

    public async Task<Response<List<Application>>> GetApplicationsByJobId(Guid jobId, CancellationToken cancellationToken)
    {
        var applications = await dbContext.Applications
        .Where(a => a.JobId == jobId).ToListAsync(cancellationToken);
        return applications is null ?
        Response<List<Application>>.Failure(new Error(ErrorTypes.NotFound, "no application for job")) :
        Response<List<Application>>.Success(applications);
    }

    public async Task<Response<bool>> ChangeApplicationStatus(Guid applicationId, Status status, CancellationToken cancellationToken)
    {
        var application = await dbContext.Applications.FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);
        if (application is null)
            return Response<bool>.Failure(new Error(ErrorTypes.NotFound, "application not found"));
        application.ChangeStatus(status);
        if (await dbContext.SaveChangesAsync(cancellationToken) <= 0)
            return Response<bool>.Failure(new Error(ErrorTypes.Internal, "internal server error"));
        return Response<bool>.Success();
    }

}
