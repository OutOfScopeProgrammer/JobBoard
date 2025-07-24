namespace JobBoard.Domain.Entities;

public class Cv : Auditable
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int ExpectedSalary { get; set; }
    public string? ImageUrl { get; set; }

    public Guid UserId { get; set; }
}
