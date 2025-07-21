namespace JobBoard.Shared.Domain.Entities;

public class Cv : Auditable
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int Salary { get; set; }
    public User User { get; set; } = new();
    public Guid UserId { get; set; }
}
