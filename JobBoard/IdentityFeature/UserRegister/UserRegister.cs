using JobBoard.IdentityFeature.Dtos;
using JobBoard.Shared.Auth;
using JobBoard.Shared.Services;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JobBoard.Identity.UserRegister;


public record RegisterDto(string Email, string Name, string Password, string RoleName);

public class UserRegister : IEndpointMarker
{
    public void Register(IEndpointRouteBuilder app)
        => app.MapGroup("api/identity")
        .MapPost("register",
        async ([FromBody] RegisterDto dto, AuthService authService,
                HttpContext context, IOptions<JwtSetting> jwtSetting) =>
        {
            var response = await authService.CreateUser(dto.Email, dto.Name, dto.Password, dto.RoleName.ToUpper());
            if (!response.IsSuccess)
            {
                var apiResponse = response.Errors.FirstOrDefault()?.ErrorType switch
                {
                    ErrorTypes.Conflict => Results.Conflict(response.Errors),
                    ErrorTypes.Internal => Results.InternalServerError(response.Errors),
                    _ => Results.BadRequest(response.Errors)
                };
                return apiResponse;
            }
            AuthHelper.SetTokenInCookie(context, response.Data!.AccessToken, jwtSetting.Value);
            return Results.Ok(new IdentityResponse(
                response.Data.UserName,
                response.Data.Role));
        })
        .WithTags("Identity")
        .Produces<IdentityResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .ProducesProblem(StatusCodes.Status400BadRequest);

}
