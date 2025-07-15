using JobBoard.Shared.Domain.Enums;

namespace JobBoard.Shared.Domain.Entities;

public class Application
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid JobId { get; set; }
    public Status status { get; set; }
}
