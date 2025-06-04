using System.Runtime.InteropServices.JavaScript;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using EmailService.Entities;
using EmailService.Services;

public interface IEmailServiceEventSubscriber {
    public void Start();
}

public class EmailServiceEventSubscriber(
    IConnection connection,
    ILogger<EmailServiceEventSubscriber> logger,
    EmailSenderService emailSenderService
    ) : IEmailServiceEventSubscriber {
    private readonly string _exchangeName = "email.notifications.exchange";
    private readonly string _queueName = "email.notifications";
    private readonly ILogger<EmailServiceEventSubscriber> _logger = logger;
    private readonly EmailSenderService _emailSenderService = emailSenderService;
    private IModel _channel;

    public void Start() {
        _channel = connection.CreateModel();

        _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic, durable: true);
        _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: "email.send.*");
        
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += OnMessageReceived;

        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        _logger.LogInformation("✅ Subscriber active...");
    }

    private async void OnMessageReceived(object sender, BasicDeliverEventArgs ea) {
        var routingKeyStr = ea.RoutingKey;
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        _logger.LogInformation($"📬 Received on [{routingKeyStr}]: {message}");
        if (RoutingKeyExtensions.TryParse(routingKeyStr, out var routingKey)) {
            switch (routingKey) {
                case RoutingKey.SendWelcomeEmail:
                    var welcomeMessage = JsonSerializer.Deserialize<WelcomeEmailMessage>(message);
                    var welcomeReceiver = welcomeMessage?.Receiver;
                    if (welcomeReceiver == null) {
                        _logger.LogWarning("⚠️ Receiver not found in message");
                        break;
                    }
                    await _emailSenderService.SendWelcomeEmailAsync(welcomeReceiver.Email, welcomeReceiver.Name);
                    break;
                case RoutingKey.SendReportEmail:
                    var reportMessage = JsonSerializer.Deserialize<ReportEmailMessage>(message);
                    if (reportMessage == null) {
                        _logger.LogWarning("⚠️ Receivers not found in message");
                        break;
                    }
                    await _emailSenderService.SendReportEmailAsync(reportMessage.Receivers, reportMessage.ReportUrl, "");
                    break;
            }
        }
        else {
           _logger.LogWarning($"⚠️ Unknown routing key received {routingKeyStr}");
        }
    }
}



public class WelcomeEmailMessage {
    public Receiver Receiver { get; set; }
}

public class ReportEmailMessage {
    public string ReportUrl { get; set; }
    public List<Receiver> Receivers { get; set; }
}

public enum RoutingKey {
    SendWelcomeEmail,
    SendReportEmail
}

public static class RoutingKeyExtensions
{
    private static readonly Dictionary<RoutingKey, string> _toString = new()
    {
        { RoutingKey.SendWelcomeEmail, "email.send.welcome_email"},
        { RoutingKey.SendReportEmail, "email.send.report_email"},
    };

    private static readonly Dictionary<string, RoutingKey> _fromString = _toString
        .ToDictionary(kv => kv.Value, kv => kv.Key);

    public static string ToRoutingKeyString(this RoutingKey key)
        => _toString.TryGetValue(key, out var val) ? val : key.ToString();

    public static bool TryParse(string str, out RoutingKey key)
        => _fromString.TryGetValue(str, out key);
}
