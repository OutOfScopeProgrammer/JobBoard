using JobBoard.Domain.Entities;

namespace JobBoard.JobFeatures.Mapper;

public record JobDto(Guid Id, string Title, string Description, int Salary);
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
