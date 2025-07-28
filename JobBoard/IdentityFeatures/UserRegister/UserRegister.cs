using JobBoard.IdentityFeatures.Dtos;
using JobBoard.IdentityFeatures.Services;
using JobBoard.Infrastructure.Auth;
using JobBoard.Shared.EndpointFilters;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
namespace JobBoard.Identity.UserRegister;

public record RegisterDto(string Email, string Name, string Password, string RoleName);

public class UserRegister : IEndpointMarker
{
    public RouteHandlerBuilder Register(IEndpointRouteBuilder app)
        => app.MapGroup("api/identity")
        .MapPost("register",
        async ([FromBody] RegisterDto dto, IdentityService authService,
                HttpContext context, IOptions<JwtSetting> jwtSetting) =>
        {
            var response = await authService.CreateUser(dto.Email, dto.Name, dto.Password, dto.RoleName);
            if (!response.IsSuccess)
            {
                if (response.Errors.FirstOrDefault() == ErrorMessages.Unauthorized)
                    return Results.Unauthorized();
                else if (response.Errors.FirstOrDefault() == ErrorMessages.InvalidRole)
                    return Results.BadRequest(response.Errors);
                else if (response.Errors.FirstOrDefault() == ErrorMessages.Internal)
                    return Results.InternalServerError(response.Errors);
            }

            AuthHelper.SetTokenInCookie(context, response.Data!.AccessToken, jwtSetting.Value);

            return Results.Ok(new IdentityResponse(
                response.Data.UserName,
                response.Data.Role));
        })
        .WithTags("Identity")
        .WithDescription("ایجاد حساب کاربری")
        .WithSummary("User register")
        .AddEndpointFilter<ValidationFilter<RegisterDto>>()
        .Produces<IdentityResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .ProducesProblem(StatusCodes.Status400BadRequest);

}
