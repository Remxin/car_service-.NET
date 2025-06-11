using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WorkshopService.Data;

public class WorkshopServiceEventSubscriber : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConnection _connection;
    private IModel _channel;
    private readonly ILogger<WorkshopServiceEventSubscriber> _logger;

    private const string ExchangeName = "workshop.status.exchange";
    private const string QueueName = "workshop.status";

    public WorkshopServiceEventSubscriber(
        IServiceScopeFactory scopeFactory,
        IConnection connection,
        ILogger<WorkshopServiceEventSubscriber> logger)
    {
        _scopeFactory = scopeFactory;
        _connection = connection;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Topic, durable: true, autoDelete: true);
        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: true);
        _channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: "workshop.order.status.*");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var routingKeyStr = ea.RoutingKey;
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _logger.LogInformation("📬 Received on [{RoutingKey}]: {Message}", routingKeyStr, message);

            if (routingKeyStr == "workshop.order.status.changed")
            {
                var orderMessage = JsonSerializer.Deserialize<ChangeOrderStatusMessage>(message);
                if (orderMessage == null)
                {
                    _logger.LogWarning("Failed to deserialize message");
                    return;
                }

                if (!int.TryParse(orderMessage.OrderId, out int orderId)) {
                    _logger.LogWarning("Failed to deserialize order message");
                    return;
                }

                var order = await dbContext.ServiceOrders.FindAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found", orderMessage.OrderId);
                    return;
                }

                order.Status = orderMessage.Status;
                await dbContext.SaveChangesAsync();
                _logger.LogInformation("✅ Order {OrderId} status updated to {Status}", order.Id, order.Status);
            }
        };

        _channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
        _logger.LogInformation("✅ Subscriber started");
        return Task.CompletedTask;
    }
}

public class ChangeOrderStatusMessage {
    public string OrderId { get; set; }
    public string Status { get; set; }
}