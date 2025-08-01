using JobBoard.Infrastructure.Persistence.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace JobBoard.Infrastructure.Persistence.RabbitMq.Configuration;

public class RabbitMqFactory : IDisposable
{
    private ConnectionFactory _factory;
    private IConnection? _connection;
    public RabbitMqFactory(IOptions<RabbitMqSetting> option)
    {
        var rabbitMqSetting = option.Value;
        if (rabbitMqSetting is null)
            throw new Exception("RabbitMq setting is empty");
        _factory = new ConnectionFactory
        {
            HostName = rabbitMqSetting.Host,
            Port = rabbitMqSetting.Port,
            UserName = rabbitMqSetting.Username,
            Password = rabbitMqSetting.Password,

        };
    }

    public async Task<IConnection> GetConnection()
        => _connection = await _factory.CreateConnectionAsync();


    public void Dispose() => _connection?.Dispose();
}
