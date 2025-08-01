using System.Diagnostics;
using System.Text;
using JobBoard.Infrastructure.Persistence.RabbitMq.Configuration;
using RabbitMQ.Client;

namespace JobBoard.Infrastructure.Persistence.RabbitMq.Producers;

public class EmailProducer(RabbitMqFactory factory)
{

    public async Task TestPublish(string message)
    {
        Debug.WriteLine("Producer begins...");
        using var connection = await factory.GetConnection();
        using var channel = await connection.CreateChannelAsync();
        var body = Encoding.UTF8.GetBytes(message);
        await channel.QueueDeclareAsync(queue: "Email", durable: false, exclusive: false,
         autoDelete: false, arguments: null);
        Debug.WriteLine("Producer Queue Declared...");
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "Email", body: body);
        Debug.WriteLine("Producer end...");

    }
}
