using JobBoard.IdentityFeatures.Dtos;
using JobBoard.Infrastructure.Auth;
using JobBoard.Shared.EndpointFilters;
using JobBoard.Shared.Services;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JobBoard.IdentityFeature.UserLogin;


public record LoginDto(string Email, string Password);
public class UserLogin : IEndpointMarker
{
    public void Register(IEndpointRouteBuilder app)
     => app.MapGroup("api/indentity")
     .MapPost("login", async ([FromBody] LoginDto dto, AuthService authServie,
      HttpContext context, IOptions<JwtSetting> jwtSetting) =>
     {
         var response = await authServie.Login(dto.Email, dto.Password);
         if (!response.IsSuccess & response.Errors.FirstOrDefault()!.ErrorType == ErrorTypes.Unauthorized)
             return Results.Unauthorized();

         AuthHelper.SetTokenInCookie(context, response.Data!.AccessToken, jwtSetting.Value);
         return Results.Ok(new IdentityResponse(response.Data!.UserName, response.Data.Role));
     })
     .WithTags("Identity")
     .Produces<IdentityResponse>(StatusCodes.Status200OK)
     .Produces(StatusCodes.Status401Unauthorized)
     .AddEndpointFilter<LogginFilter>();
}
