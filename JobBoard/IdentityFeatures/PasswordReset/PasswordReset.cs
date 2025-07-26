using JobBoard.IdentityFeatures.Dtos;
using JobBoard.IdentityFeatures.Services;
using JobBoard.Infrastructure.Auth;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JobBoard.IdentityFeature.PasswordReset;

public record ResetDto(string Email, string OldPassword, string NewPassword);
public class PasswordReset : IEndpointMarker
{
    public RouteHandlerBuilder Register(IEndpointRouteBuilder app)
        => app.MapGroup("api/identity")
        .MapPost("reset", async ([FromBody] ResetDto dto, IdentityService authService,
         HttpContext context, IOptions<JwtSetting> jwtSetting) =>
        {
            var response = await authService.ResetPassword(dto.Email, dto.OldPassword, dto.NewPassword);
            if (!response.IsSuccess)
            {
                if (response.Errors.FirstOrDefault() == ErrorMessages.Unauthorized)
                    return Results.Unauthorized();
                else if (response.Errors.FirstOrDefault() == ErrorMessages.Internal)
                    return Results.InternalServerError();
            }

            AuthHelper.SetTokenInCookie(context, response.Data!.AccessToken, jwtSetting.Value);

            return Results.Ok(new IdentityResponse(response.Data.UserName, response.Data.Role));

        }).WithTags("Identity")
        .WithDescription("تغییر رمز عبور")
        .WithSummary("Password reset")
        .Produces<IdentityResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .Produces(StatusCodes.Status401Unauthorized);
}
