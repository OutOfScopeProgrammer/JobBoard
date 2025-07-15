namespace JobBoard.Shared.Domain.Entites;

public class Job
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public User User { get; set; } = new();
    public Guid userId { get; set; }
}
