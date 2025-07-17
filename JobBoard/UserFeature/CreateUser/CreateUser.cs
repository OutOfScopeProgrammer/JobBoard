using JobBoard.Shared.Utilities;

namespace JobBoard.UserFeature.CreateUser;

public class CreateUser : IEndpointMarker
{
    public void Register(IEndpointRouteBuilder app)
        => app.MapGroup("identity")
        .MapPost("register", () =>
        {
            return Results.Ok("hit");
        });
}
