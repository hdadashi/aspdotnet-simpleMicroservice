using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Payment.Application.EventBus
{
    public class MessagePublisher : IMessagePublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchange = "payment.events";

        public MessagePublisher(IConfiguration config)
        {
            var factory = new ConnectionFactory
            {
                HostName = config["RabbitMQ:Host"] ?? "localhost",
                UserName = config["RabbitMQ:User"] ?? "guest",
                Password = config["RabbitMQ:Pass"] ?? "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_exchange, ExchangeType.Fanout, durable: true);
        }

        public void PublishPaymentProcessed(PaymentProcessedEvent ev)
        {
            var json = JsonSerializer.Serialize(ev);
            var body = Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(_exchange, "", null, body);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}