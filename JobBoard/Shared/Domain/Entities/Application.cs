using JobBoard.Shared.Domain.Enums;

namespace JobBoard.Shared.Domain.Entities;

public class Application
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid ApplicantId { get; set; }
    public Guid JobId { get; set; }
    public Status Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime StatusChangedAt { get; set; }
    public Application() { }
    private Application(string description, Guid jobId, Guid applicantId, Status status)
    {
        Description = description;
        JobId = jobId;
        ApplicantId = applicantId;
        Status = status;
    }


    public static Application Create(string description, Guid jobId, Guid applicantId, Status status)
        => new(description, jobId, applicantId, status);
}
