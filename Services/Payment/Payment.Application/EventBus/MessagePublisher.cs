using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Payment.Application.EventBus
{
    public class MessagePublisher : IMessagePublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<MessagePublisher> _logger;
        private const string _exchange = "payment.events";

        public MessagePublisher(IConfiguration config, ILogger<MessagePublisher> logger)
        {
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = config["RabbitMQ:Host"] ?? "localhost",
                UserName = config["RabbitMQ:User"] ?? "guest",
                Password = config["RabbitMQ:Pass"] ?? "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_exchange, ExchangeType.Fanout, durable: true);
            _logger.LogInformation("Connected to RabbitMQ and declared exchange {Exchange}", _exchange);
        }

        public void PublishPaymentProcessed(PaymentProcessedEvent ev)
        {
            var json = JsonSerializer.Serialize(ev);
            var body = Encoding.UTF8.GetBytes(json);

            // Fanout = همه subscriberها دریافت می‌کنند
            _channel.BasicPublish(exchange: _exchange, routingKey: "", basicProperties: null, body: body);
            _logger.LogInformation("📤 Published PaymentProcessedEvent → {Exchange}", _exchange);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}