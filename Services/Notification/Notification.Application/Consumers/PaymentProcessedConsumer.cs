using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Notification.Domain.Events;

namespace Notification.Application.Consumers;

public class PaymentProcessedConsumer(ILogger<PaymentProcessedConsumer> logger, IConfiguration config)
    : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var host = config["RabbitMQ:Host"] ?? "localhost";
        var user = config["RabbitMQ:User"] ?? "guest";
        var pass = config["RabbitMQ:Pass"] ?? "guest";

        logger.LogInformation("Connecting to RabbitMQ at {Host}...", host);

        var factory = new ConnectionFactory
        {
            HostName = host,
            UserName = user,
            Password = pass
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("payment.events", ExchangeType.Fanout, durable: true);
        _channel.QueueDeclare("PaymentProcessedEvent", durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind("PaymentProcessedEvent", "payment.events", "");

        logger.LogInformation("Connected to RabbitMQ and listening to PaymentProcessedEvent...");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var evt = JsonSerializer.Deserialize<PaymentProcessedEvent>(json);

            logger.LogInformation("Received PaymentProcessedEvent: Token={Token}, Amount={Amount}, Status={Status}",
                evt?.Token, evt?.Amount, evt?.Status);

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume("PaymentProcessedEvent", autoAck: false, consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
