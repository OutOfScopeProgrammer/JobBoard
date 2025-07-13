using JobBoard.Shared.Auth;

namespace JobBoard.Shared.Extensions;

public static class ServiceCollections
{
    public static void AddProjectDependecy(this IServiceCollection services, IConfiguration configuration)
    {
        services.IdentityServices(configuration);
    }



    private static void IdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtSetting>().Bind<JwtSetting>(config: configuration.GetSection("JwtSetting"));


    }

}
