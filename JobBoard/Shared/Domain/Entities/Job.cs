namespace JobBoard.Shared.Domain.Entities;

public class Job : Auditable
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Salary { get; set; }
    public List<Application> Applications { get; set; } = [];
    public DateTime ClosedAt { get; set; }

    public Guid EmployeeId { get; set; }

    public Job() { }
    private Job(string title, string description, Guid employeeId)
    {
        Title = title;
        Description = description;
        EmployeeId = employeeId;
    }


    public void ApplyToJob(Application application)
    {
        Applications.Add(application);
    }
    public void updateJob(string? title, string? description)
    {
        Title = title ?? Title;
        Description = description ?? Description;
    }


    public static Job Create(string title, string description, Guid employeeId)
        => new(title, description, employeeId);
}
