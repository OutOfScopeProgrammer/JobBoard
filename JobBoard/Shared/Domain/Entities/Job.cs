namespace JobBoard.Shared.Domain.Entities;

public class Job
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Application> Applications { get; set; } = [];
    public Guid EmployeeId { get; set; }

    public Job() { }
    private Job(string title, string description, Guid employeeId)
    {
        Title = title;
        Description = description;
        EmployeeId = employeeId;
    }
    public void updateJob(string? title, string? description)
    {
        Title = title ?? Title;
        Description = description ?? Description;
    }


    public static Job Create(string title, string description, Guid employeeId)
        => new(title, description, employeeId);
}
