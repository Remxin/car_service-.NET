using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;

namespace ReportService.Services;

public interface IWorkshopStatusPublisher {
    void PublishStatusChange(string orderId, string status);
}

public class WorkshopStatusPublisher : IWorkshopStatusPublisher
{
    private readonly IConnection _connection;
    private readonly ILogger<WorkshopStatusPublisher> _logger;
    private const string ExchangeName = "workshop.status.exchange";

    public WorkshopStatusPublisher(IConnection connection, ILogger<WorkshopStatusPublisher> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public void PublishStatusChange(string orderId, string status)
    {
        using var channel = _connection.CreateModel();

        var message = new ChangeOrderStatusMessage
        {
            OrderId = orderId,
            Status = status
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Topic, durable: true, autoDelete: true);

        var routingKey = "workshop.order.status.changed";
        channel.BasicPublish(
            exchange: ExchangeName,
            routingKey: routingKey,
            basicProperties: null,
            body: body
        );

        _logger.LogInformation("📤 Sent status change for order {OrderId} with status {Status}", orderId, status);
    }

    private class ChangeOrderStatusMessage
    {
        public string OrderId { get; set; }
        public string Status { get; set; }
    }
}