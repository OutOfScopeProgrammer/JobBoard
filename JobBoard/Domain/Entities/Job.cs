namespace JobBoard.Domain.Entities;

public class Job : Auditable
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Salary { get; set; }
    public List<Application> Applications { get; set; } = [];
    public DateTime ClosedAt { get; set; }

    public Guid EmployeeId { get; set; }
    public byte[] ConcurrencyToken { get; set; } = Guid.NewGuid().ToByteArray();

    public Job() { }
    private Job(string title, string description, Guid employeeId, int salary)
    {
        Title = title;
        Description = description;
        EmployeeId = employeeId;
        Salary = salary;
    }


    public void ApplyToJob(Application application)
    {
        Applications.Add(application);
    }
    public void UpdateJob(string? title, string? description)
    {
        Title = title ?? Title;
        Description = description ?? Description;
    }


    public static Job Create(string title, string description, Guid employeeId, int salary)
        => new(title, description, employeeId, salary);
}
