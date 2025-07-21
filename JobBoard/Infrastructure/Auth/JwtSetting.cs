namespace JobBoard.Infrastructure.Auth;

public record class JwtSetting
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string Secret { get; set; }
    public required int ExpirationInMinute { get; set; }
}
