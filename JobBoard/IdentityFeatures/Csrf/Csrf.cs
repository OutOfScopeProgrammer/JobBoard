using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Antiforgery;

namespace JobBoard.IdentityFeatures.Csrf;

public class Csrf : IEndpointMarker
{
    public void Register(IEndpointRouteBuilder app)
        => app.MapGroup("api/identity")
        .MapGet("csrf-token", (IAntiforgery antiforgery, HttpContext context) =>
        {
            var token = antiforgery.GetAndStoreTokens(context);
            return Results.Ok(new { token = token.RequestToken });
        }).RequireAuthorization();
}
