
namespace JobBoard.Shared.Domain.Entities;

public abstract class Auditable
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
