namespace JobBoard.Domain.Entities;

public class Cv : Auditable
{
    public Cv() { }
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int ExpectedSalary { get; set; }
    public string? ImageUrl { get; set; }

    public Guid UserId { get; set; }
    public byte[] ConcurrencyToken { get; set; } = Guid.NewGuid().ToByteArray();




    public void SetImage(string url) => ImageUrl = url;

    private Cv(string fullName, string? fullAddress,
     string city, int expectedSalary, Guid userId)
    {
        FullName = fullName;
        FullAddress = fullAddress ?? string.Empty;
        City = city;
        ExpectedSalary = expectedSalary;
        UserId = userId;
    }

    public static Cv Create(string fullName, string? fullAddress,
     string city, int expectedSalary, Guid userId)
        => new(fullName, fullAddress, city, expectedSalary, userId);

}
