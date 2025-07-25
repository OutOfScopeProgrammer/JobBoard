using JobBoard.IdentityFeatures.Dtos;
using JobBoard.IdentityFeatures.Services;
using JobBoard.Infrastructure.Auth;
using JobBoard.Shared.EndpointFilters;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JobBoard.IdentityFeature.UserLogin;


public record LoginDto(string Email, string Password);
public class UserLogin : IEndpointMarker
{
    public RouteHandlerBuilder Register(IEndpointRouteBuilder app)
     => app.MapGroup("api/indentity")
     .MapPost("login", async ([FromBody] LoginDto dto, IdentityService authServie,
      HttpContext context, IOptions<JwtSetting> jwtSetting) =>
     {
         var response = await authServie.Login(dto.Email, dto.Password);
         if (!response.IsSuccess)
             return Results.Unauthorized();

         AuthHelper.SetTokenInCookie(context, response.Data!.AccessToken, jwtSetting.Value);
         return Results.Ok(new IdentityResponse(response.Data!.UserName, response.Data.Role));
     })
     .WithTags("Identity")
     .WithDescription("ورود به حساب کاربری")
     .WithSummary("User login")
     .Produces<IdentityResponse>(StatusCodes.Status200OK)
     .Produces(StatusCodes.Status401Unauthorized);
}
