using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public interface IReportServiceEventSubscriber {
    public void Start();
}

public class ReportServiceEventSubscriber : IReportServiceEventSubscriber {
    private readonly string _exchangeName = "workshop.events.exchange";
    private readonly string _queueName = "workshop.events";
    private IConnection _connection;
    private IModel _channel;

    public ReportServiceEventSubscriber(IConnection connection) {
        _connection = connection;
    }
    
    public void Start() {
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic, durable: true);
        _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: "report.*");
        
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += OnMessageReceived;

        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        Console.WriteLine("✅Subsriber active...");
    }

    private void OnMessageReceived(object sender, BasicDeliverEventArgs ea) {
        var routingKey = ea.RoutingKey;
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        Console.WriteLine($"📬 Received on [{routingKey}]: {message}");

        switch (routingKey)
        {
            case "report.created":
         
                break;
            case "report.updated":
                
                break;
            default:
                Console.WriteLine("⚠️ Unknown event - cannot proceed.");
                break;
        }
    }
}

public class ReportCreatedMessage
{
    public Guid ReportId { get; set; }
    public string RequestedBy { get; set; }
    public DateTime RequestedAt { get; set; }
}