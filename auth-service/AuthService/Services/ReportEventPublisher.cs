using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace AuthService.Services;

public interface IWorkshopEventPublisher {
    void PublishEvent(string routingKey, string message);
    void PublishEvent<T>(string routingKey, T message);
}

public class ReportEventPublisher : IWorkshopEventPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string ExchangeName = "workshop.events.exchange";

    public ReportEventPublisher(IConnection connection)
    {
        _connection = connection;
        _channel = connection.CreateModel();
        
        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Topic, durable: true);
    }

    public void PublishEvent(string routingKey, string message) {
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: ExchangeName,
            routingKey: routingKey,
            basicProperties: properties,
            body: body);
    }

    public void PublishEvent<T>(string routingKey, T message) {
        var json = JsonSerializer.Serialize(message);
        PublishEvent(routingKey, json);
    }

    public void Dispose() {
        _channel?.Close();
        _channel?.Dispose();
    }
}
