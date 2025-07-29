using System.Reflection;
using System.Text;
using FluentValidation;
using JobBoard.CvFeatures.Services;
using JobBoard.Domain.Entities;
using JobBoard.FileFeatures;
using JobBoard.IdentityFeatures.Services;
using JobBoard.Infrastructure.Auth;
using JobBoard.Infrastructure.ExceptionHandlers;
using JobBoard.Infrastructure.Middlewares;
using JobBoard.Infrastructure.Persistence.Intercepters;
using JobBoard.JobApplicationFeatures.Services;
using JobBoard.JobFeatures.Services;
using JobBoard.Shared.Persistence;
using JobBoard.Shared.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;

namespace JobBoard.Infrastructure.Extensions;

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
        services.AddScoped<IdentityService>();
        services.AddScoped<JobService>();
        services.AddScoped<JobApplicationService>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<CvService>();
        services.AddScoped<LogMiddleware>();
        services.AddScoped<GlobalExceptionHandler>();
        services.AddScoped<ImageProcessor>();
    }

    private static void ApplicationPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        if (connectionString is null) throw new Exception("Connection string problem");
        services.AddScoped<IInterceptor, AuditIntercepter>();
        services.AddScoped<IInterceptor, ConcurrencyInterceptor>();


        services.AddDbContext<AppDbContext>((provider, option) =>
        {
            var intercepters = provider.GetServices<IInterceptor>() ??
             throw new Exception("interceptors is not configured");

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
            option.AddPolicy(AuthPolicy.AdminOnly, policy => policy.RequireRole(ApplicationRoles.ADMIN.ToString()));
            option.AddPolicy(AuthPolicy.ApplicantOnly, policy => policy.RequireRole(ApplicationRoles.APPLICANT.ToString()));
            option.AddPolicy(AuthPolicy.EmployeeOnly, policy => policy.RequireRole(ApplicationRoles.EMPLOYEE.ToString()));

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
            var routBuilder = instance?.Register(app);
            routBuilder?.DisableAntiforgery();
        }
    }

}
