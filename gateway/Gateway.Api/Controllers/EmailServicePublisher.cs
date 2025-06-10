namespace Gateway.Api.Controllers;

using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

public interface IEmailServiceEventPublisher
{
    void PublishEvent(string routingKey, string message);
    void PublishEvent<T>(string routingKey, T message);
}

public class EmailServicePublisher : IEmailServiceEventPublisher {
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string ExchangeName = "email.notifications.exchange";
    
    public EmailServicePublisher(IConnection connection)
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