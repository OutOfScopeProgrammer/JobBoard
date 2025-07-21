using System.Reflection;
using System.Text;
using JobBoard.JobFeatures.Services;
using JobBoard.Shared.Auth;
using JobBoard.Shared.Domain.Entities;
using JobBoard.Shared.Persistence;
using JobBoard.Shared.Persistence.Intercepters;
using JobBoard.Shared.Services;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;

namespace JobBoard.Shared.Extensions;

public static class ServiceCollections
{
    public static void AddProjectDependecy(this IServiceCollection services, IConfiguration configuration)
    {
        services.IdentityServices(configuration);
        services.ApplicationServices();
        services.ApplicationPersistence(configuration);
    }



    private static void ApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<TokenProvider>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<AuthService>();
        services.AddScoped<JobService>();

    }

    private static void ApplicationPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        if (connectionString is null) throw new Exception("Connection string problem");
        services.AddScoped<IInterceptor, AuditIntercepter>();

        services.AddDbContext<AppDbContext>((provider, option) =>
        {
            var intercepters = provider.GetServices<IInterceptor>();
            if (intercepters is null) throw new Exception("interceptors is not configured");
            option.UseNpgsql(connectionString);
            option.EnableSensitiveDataLogging();
            option.AddInterceptors(intercepters);
        });

    }

    private static void IdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtSetting>().Bind(config: configuration.GetSection("JwtSetting"));
        services.AddAuthorization(option =>
        {
            option.AddPolicy("AdminOnly", policy => policy.RequireRole("ADMIN"));
            option.AddPolicy("ApplicantOnly", policy => policy.RequireRole("APPLICANT"));
            option.AddPolicy("EmployeeOnly", policy => policy.RequireRole("EMPLOYEE"));

        });
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
    }

    public static void MapApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var iEndpoint = typeof(IEndpointMarker);
        var endpoints = assembly.GetTypes()
        .Where(t => t is { IsClass: true, IsAbstract: false } & iEndpoint.IsAssignableFrom(t));
        foreach (var endppoint in endpoints)
        {
            var instance = Activator.CreateInstance(endppoint) as IEndpointMarker;
            instance?.Register(app);
        }
    }

}
