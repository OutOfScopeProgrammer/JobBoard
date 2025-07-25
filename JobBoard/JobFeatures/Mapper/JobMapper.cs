using JobBoard.Domain.Entities;
using JobBoard.JobFeatures.Dtos;
using JobBoard.JobFeatures.Services;

namespace JobBoard.JobFeatures.Mapper;

public static class JobMapper
{

    public static JobDto MapToJobDto(Job job)
    {
        return new(job.Id, job.Title, job.Description, job.Salary);
    }

    public static List<JobDto> MapToJobDto(List<Job> jobs)
    {
        List<JobDto> dtos = [];
        foreach (var job in jobs)
        {
            dtos.Add(MapToJobDto(job));
        }
        return dtos;
    }


}
