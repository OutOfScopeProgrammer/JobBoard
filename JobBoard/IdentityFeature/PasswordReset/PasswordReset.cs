using JobBoard.IdentityFeature.Dtos;
using JobBoard.Shared.Auth;
using JobBoard.Shared.Services;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JobBoard.IdentityFeature.PasswordReset;

public record ResetDto(string Email, string OldPassword, string NewPassword);
public class PasswordReset : IEndpointMarker
{
    public void Register(IEndpointRouteBuilder app)
        => app.MapGroup("api/identity")
        .MapPost("reset", async ([FromBody] ResetDto dto, AuthService authService,
         HttpContext context, IOptions<JwtSetting> jwtSetting) =>
        {
            var response = await authService.ResetPassword(dto.Email, dto.OldPassword, dto.NewPassword);
            if (!response.IsSuccess)
            {
                var apiResponse = response.Errors.FirstOrDefault()!.ErrorType switch
                {
                    ErrorTypes.Internal => Results.InternalServerError(response.Errors),
                    ErrorTypes.Unauthorized => Results.Unauthorized()
                };
                return apiResponse;
            }
            CookieHelper.SetTokenInCookie(context, response.Data!.AccessToken, jwtSetting.Value);
            return Results.Ok(new IdentityResponse(response.Data.UserName, response.Data.Role));

        }).WithTags("Identity")
        .Produces<IdentityResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .Produces(StatusCodes.Status401Unauthorized);
}
