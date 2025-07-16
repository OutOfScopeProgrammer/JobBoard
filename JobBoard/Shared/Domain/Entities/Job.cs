namespace JobBoard.Shared.Domain.Entities;

public class Job
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Application> Applications { get; set; } = [];
    public Guid EmployeeId { get; set; }
}
