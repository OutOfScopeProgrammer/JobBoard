using JobBoard.Shared.Utilities;

namespace JobBoard.IdentityFeature.UserLogin;


public record LoginDto(string Email, string Password);
public class UserLogin : IEndpointMarker
{
    public void Register(IEndpointRouteBuilder app)
     => app.MapGroup("api/indentity")
     .MapPost("login", () => { });
}
