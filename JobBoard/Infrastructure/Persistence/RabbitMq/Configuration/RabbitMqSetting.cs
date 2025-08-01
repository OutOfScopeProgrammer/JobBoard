namespace JobBoard.Infrastructure.Persistence.Extensions.Configuration;

public class RabbitMqSetting
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string QueueName { get; set; }



}
