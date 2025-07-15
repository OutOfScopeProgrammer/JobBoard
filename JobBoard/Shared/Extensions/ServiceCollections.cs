using System.Text;
using JobBoard.Shared.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace JobBoard.Shared.Extensions;

public static class ServiceCollections
{
    public static void AddProjectDependecy(this IServiceCollection services, IConfiguration configuration)
    {
        services.IdentityServices(configuration);
        services.ApplicationServices();
    }



    private static void ApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<TokenProvider>();
    }

    private static void IdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtSetting>().Bind<JwtSetting>(config: configuration.GetSection("JwtSetting"));
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var token = context.HttpContext.Request.Cookies["access_token"];
                    context.Token = token;
                    return Task.CompletedTask;
                }
            };

            var jwtOption = configuration.GetSection("JwtSetting").Get<JwtSetting>()
             ?? throw new Exception("Something is wrong with Jwt token setting");


            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.FromMinutes(1),
                ValidIssuer = jwtOption.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOption.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.Secret))
            };

        });
        services.AddAuthorization();


    }

}
