
using JobBoard.Infrastructure.Persistence.Extensions.Configuration;
using JobBoard.Infrastructure.Persistence.RabbitMq.Configuration;
using JobBoard.Infrastructure.Persistence.RabbitMq.Producers;
using JobBoard.Infrastructure.Persistence.RabbitMq.Receiver;

namespace JobBoard.Infrastructure.Extensions;

public static class RabbitMqExtension
{

    public static void AddRAbbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RabbitMqSetting>().Bind(configuration.GetSection("RabbitMq"));
        services.AddSingleton<RabbitMqFactory>();
        services.AddScoped<EmailProducer>();
        services.AddScoped<EmailReceiver>();

    }
}
