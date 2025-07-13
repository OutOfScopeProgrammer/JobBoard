namespace JobBoard.Shared.Domain;

public class Role
{
    public Guid Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
