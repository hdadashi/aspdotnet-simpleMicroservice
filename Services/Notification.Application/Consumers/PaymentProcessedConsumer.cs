using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notification.Domain.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Notification.Application.Consumers
{
    public class PaymentProcessedConsumer : BackgroundService
    {
        private readonly ILogger<PaymentProcessedConsumer> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string _exchange = "payment.events";
        private const string _queue = "PaymentProcessedEvent";

        public PaymentProcessedConsumer(ILogger<PaymentProcessedConsumer> logger)
        {
            _logger = logger;
            _logger.LogInformation("Connecting to RabbitMQ at localhost...");

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Exchange و Queue و Binding
            _channel.ExchangeDeclare(_exchange, ExchangeType.Fanout, durable: true);
            _channel.QueueDeclare(_queue, durable: false, exclusive: false, autoDelete: false);
            _channel.QueueBind(_queue, _exchange, "");

            _logger.LogInformation("Connected. Queue {Queue} bound to exchange {Exchange}", _queue, _exchange);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Listening for PaymentProcessedEvent messages...");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                try
                {
                    var evt = JsonSerializer.Deserialize<PaymentProcessedEvent>(json);
                    if (evt != null)
                    {
                        _logger.LogInformation("Received PaymentProcessedEvent!");
                        _logger.LogInformation("Token: {Token}", evt.Token);
                        _logger.LogInformation("Amount: {Amount}", evt.Amount);
                        _logger.LogInformation("Status: {Status}", evt.Status);
                        _logger.LogInformation("Rrn: {Rrn}", evt.Rrn);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process message: {Json}", json);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(_queue, autoAck: false, consumer);

            // نگه داشتن Worker فعال
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
