using JobBoard.Shared.Utilities;

namespace JobBoard.UserFeature.ApplyToJob;

public class ApplyToJob : IEndpointMarker
{
    public void Register(IEndpointRouteBuilder app)
     => app.MapGroup("user")
     .MapPost("application", () =>
     {

     });
}
