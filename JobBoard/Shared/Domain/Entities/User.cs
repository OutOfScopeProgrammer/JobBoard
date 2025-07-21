namespace JobBoard.Shared.Domain.Entities;

public class User : Auditable
{
    public User() { }

    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<Job>? Jobs { get; set; }
    public List<Application>? Applications { get; set; }

    public Role Role { get; set; } = new();
    public Guid RoleId { get; set; }

    private User(string email, string name)
    {
        Email = email;
        Name = name;
    }
    public void SetHashedPassword(string hashedPassword) => HashedPassword = hashedPassword;
    public void SetRole(Role role)
    {
        Role = role;
        RoleId = role.Id;
    }
    public static User Create(string email, string name) => new(email, name);

}
