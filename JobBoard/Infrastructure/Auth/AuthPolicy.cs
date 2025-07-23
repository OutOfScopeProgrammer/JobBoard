namespace JobBoard.Infrastructure.Auth;

public static class AuthPolicy
{
    public static string ApplicantOnly { get; set; } = "ApplicantOnly";
    public static string AdminOnly { get; set; } = "AdminOnly";
    public static string EmployeeOnly { get; set; } = "EmployeeOnly";
}
