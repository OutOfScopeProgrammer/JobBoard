using System.Diagnostics;
using System.Text;
using JobBoard.Infrastructure.Persistence.Extensions.Configuration;
using JobBoard.Infrastructure.Persistence.RabbitMq.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace JobBoard.Infrastructure.Persistence.RabbitMq.Receiver;

public class EmailReceiver(RabbitMqFactory factory)
{

    public async Task TestReceiver()
    {
        Debug.WriteLine("Receiver begins...");

        using var connection = await factory.GetConnection();
        using var channel = await connection.CreateChannelAsync();
        Debug.WriteLine("Receiver Queue Declared...");
        await channel.QueueDeclareAsync(queue: "Email", durable: false, exclusive: false, autoDelete: false,
    arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Debug.WriteLine($"the message received: {message}");

            return Task.CompletedTask;
        };
        var message = await channel.BasicConsumeAsync("Email", autoAck: true, consumer: consumer);

        Debug.WriteLine("Receiver end...");

    }
}
