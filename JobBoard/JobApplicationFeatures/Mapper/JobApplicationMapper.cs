using JobBoard.Domain.Entities;

namespace JobBoard.JobApplicationFeatures.Mapper;


public record ApplicationDto(Guid Id, string Description, string Status);
public static class JobApplicationMapper
{

    public static ApplicationDto MapToApplicationDto(Application application)
        => new(application.Id, application.Description, application.Status.ToString());

    public static List<ApplicationDto> MapToApplicationDto(List<Application> applications)
    {
        var dtos = new List<ApplicationDto>();
        foreach (var application in applications)
        {
            dtos.Add(MapToApplicationDto(application));
        }
        return dtos;
    }
}
