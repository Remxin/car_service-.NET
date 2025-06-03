using System.Runtime.InteropServices.JavaScript;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using MongoDB.Driver;
using ReportService.Data;
using ReportService.Entities;
using ReportService.Services;
using ReturnDocument = MongoDB.Driver.ReturnDocument;

public interface IReportServiceEventSubscriber {
    public void Start();
}

public class ReportServiceEventSubscriber : IReportServiceEventSubscriber {
    private readonly string _exchangeName = "workshop.events.exchange";
    private readonly string _queueName = "workshop.events";
    private IConnection _connection;
    private MongoDbContext _dbContext;
    private IModel _channel;

    public ReportServiceEventSubscriber(IConnection connection, MongoDbContext dbContext) {
        _connection = connection;
        _dbContext = dbContext;
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

    private async void OnMessageReceived(object sender, BasicDeliverEventArgs ea) {
        var routingKeyStr = ea.RoutingKey;
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        Console.WriteLine($"📬 Received on [{routingKeyStr}]: {message}");


        if (RoutingKeyExtensions.TryParse(routingKeyStr, out var routingKey)) {
            switch (routingKey) {
                case RoutingKey.UserCreated:
                    var user = JsonSerializer.Deserialize<UserEntity>(message);
                    if (user == null) {
                        throw new ArgumentNullException(nameof(user), "user not provided");
                    }

                    user.UserRoles = [];
;                    await _dbContext.Users.InsertOneAsync(user);
                    break;
                case RoutingKey.UserRoleAdded:
                    var addRoleMessage = JsonSerializer.Deserialize<UserRoleAddedMessage>(message);
                    if (addRoleMessage == null) {
                        throw new ArgumentNullException(nameof(addRoleMessage), "user permission not provided");
                    }
                    var filterRoleAdd = Builders<UserEntity>.Filter.Eq(u => u.Id, addRoleMessage.UserId);
                    var updateRoleAdd = Builders<UserEntity>.Update.AddToSet(u => u.UserRoles, addRoleMessage.Role);

                    var updatedRoleAddUser = await _dbContext.Users.FindOneAndUpdateAsync(
                        filterRoleAdd,
                        updateRoleAdd,
                        new FindOneAndUpdateOptions<UserEntity> {
                            ReturnDocument = ReturnDocument.After
                        });

                    if (updatedRoleAddUser == null) {
                        throw new MongoInternalException("User not found or update failed.");
                    }
                    break;
                case RoutingKey.UserRoleRemoved:
                    var removeRoleMessage = JsonSerializer.Deserialize<UserRoleRemovedMessage>(message);
                    if (removeRoleMessage == null) {
                        throw new ArgumentNullException(nameof(removeRoleMessage), "user permission not provided");
                    }

                    var filter = Builders<UserEntity>.Filter.Eq(u => u.Id, removeRoleMessage.UserId);
                    var update = Builders<UserEntity>.Update.Pull(u => u.UserRoles, removeRoleMessage.Role);

                    var updatedUser = await _dbContext.Users.FindOneAndUpdateAsync(
                        filter,
                        update,
                        new FindOneAndUpdateOptions<UserEntity> {
                            ReturnDocument = ReturnDocument.After
                        });

                    if (updatedUser == null) {
                        throw new MongoInternalException("User not found or update failed.");
                    }

                    break;
                
                case RoutingKey.OrderCreated:
                    var order = JsonSerializer.Deserialize<ReportCreatedMessage>(message);
                    if (order == null) {
                        throw new ArgumentNullException(nameof(order), "order was not provided");
                    }
                    var foundUser = await _dbContext.Users.Find(u => u.Id == order.UserId).FirstOrDefaultAsync();
                    if (foundUser == null) {
                        throw new MongoInternalException("User not found or update failed.");
                    }
                    var report = new ReportEntity {
                        OrderId = order.Id,
                        Vehicle = order.Vehicle,
                        ReportUrl = "",
                        ExpiresAt = DateTime.Now,
                        Status = order.Status,
                        User = foundUser,
                        CreatedAt = order.CreatedAt,
                    };
                    await _dbContext.Reports.InsertOneAsync(report);
                    break;

                case RoutingKey.TaskAdded:
                    var task = JsonSerializer.Deserialize<TaskCreatedMessage>(message);
                    if (task == null) {
                        throw new ArgumentNullException("task", "task was not provided");
                    }
                    var dbTask = new ServiceTaskEntity {
                        Id = task.Id,
                        Description = task.Description,
                        CreatedAt = task.CreatedAt,
                        LaborCost = task.LaborCost,
                    };
                    var taskFilter = Builders<ReportEntity>.Filter.Eq(r => r.OrderId, task.OrderId); // lub task.Id jeśli to report ID
                    var taskUpdate = Builders<ReportEntity>.Update.Push(r => r.ServiceTasks, dbTask);

                    var updatedReport = await _dbContext.Reports.FindOneAndUpdateAsync(
                        taskFilter,
                        taskUpdate,
                        new FindOneAndUpdateOptions<ReportEntity>
                        {
                            ReturnDocument = ReturnDocument.After
                        });
                    if (updatedReport == null) {
                        throw new MongoInternalException("task update failed");
                    }
                    break;
                case RoutingKey.PartAdded:
                    var part = JsonSerializer.Deserialize<PartAddedMessage>(message);
                    if (part == null) {
                        throw new ArgumentNullException(nameof(part), "part was not provided");
                    }

                    var dbPart = new ServicePartEntity {
                        Id = part.Id,
                        VehiclePart = part.VehiclePart,
                        Quantity = part.Quantity,
                    };
                    var partFilter = Builders<ReportEntity>.Filter.Eq(r => r.OrderId, part.OrderId); // lub task.Id jeśli to report ID
                    var pUpdate = Builders<ReportEntity>.Update.Push(r => r.ServiceParts, dbPart);

                    var updatedPart = await _dbContext.Reports.FindOneAndUpdateAsync(
                        partFilter,
                        pUpdate,
                        new FindOneAndUpdateOptions<ReportEntity>
                        {
                            ReturnDocument = ReturnDocument.After
                        });
                    if (updatedPart == null) {
                        throw new MongoInternalException("task update failed");
                    }
                    break;
                case RoutingKey.CommentAdded:
                    var comment = JsonSerializer.Deserialize<CommentAddedMessage>(message);
                    if (comment == null) {
                        throw new ArgumentNullException("task", "task was not provided");
                    }

                    var dbComment = new ServiceCommentEntity {
                        Id = comment.Id,
                        Content= comment.Content,
                        CreatedAt = comment.CreatedAt,
                    };
                    var commentFilter = Builders<ReportEntity>.Filter.Eq(r => r.OrderId, comment.OrderId); // lub task.Id jeśli to report ID
                    var commentUpdate = Builders<ReportEntity>.Update.Push(r => r.ServiceComments, dbComment);

                    var updatedComment = await _dbContext.Reports.FindOneAndUpdateAsync(
                        commentFilter,
                        commentUpdate,
                        new FindOneAndUpdateOptions<ReportEntity>
                        {
                            ReturnDocument = ReturnDocument.After
                        });
                    if (updatedComment == null) {
                        throw new MongoInternalException("task update failed");
                    }
                    break;
                case RoutingKey.StatusChanged:
                    var status = JsonSerializer.Deserialize<StatusChangedMessage>(message);
                    var statusFilter = Builders<ReportEntity>.Filter.Eq(r => r.OrderId, status.OrderId); // lub task.Id jeśli to report ID
                    var statusUpdate = Builders<ReportEntity>.Update.Set(r => r.Status, statusFilter);

                    var updatedStatus = await _dbContext.Reports.FindOneAndUpdateAsync(
                        statusFilter,
                        statusUpdate,
                        new FindOneAndUpdateOptions<ReportEntity>
                        {
                            ReturnDocument = ReturnDocument.After
                        });
                    if (updatedStatus == null) {
                        throw new MongoInternalException("task update failed");
                    }
                    break;
                case RoutingKey.OrderDeleted:
                    var orderDeleted = JsonSerializer.Deserialize<OrderDeletedMessage>(message);
                    if (orderDeleted == null) throw new ArgumentNullException("orderDeleted");

                    var deleteResult = await _dbContext.Reports.DeleteOneAsync(r => r.OrderId == orderDeleted.Id);
                    if (deleteResult.DeletedCount == 0) {
                        Console.WriteLine($"⚠️ No report found to delete for order {orderDeleted.Id}");
                    }
                    break;
                case RoutingKey.CommentRemoved:
                    var commentRemoved = JsonSerializer.Deserialize<CommentRemovedMessage>(message);
                    if (commentRemoved == null) throw new ArgumentNullException("commentRemoved");

                    var commentRemoveFilter = Builders<ReportEntity>.Filter.Eq(r => r.OrderId, commentRemoved.OrderId);
                    var commentRemoveUpdate = Builders<ReportEntity>.Update.PullFilter(
                        r => r.ServiceComments, c => c.Id == commentRemoved.Id
                    );

                    await _dbContext.Reports.UpdateOneAsync(commentRemoveFilter, commentRemoveUpdate);
                    break;
                case RoutingKey.TaskRemoved:
                    var taskRemoved = JsonSerializer.Deserialize<TaskRemovedMessage>(message);
                    if (taskRemoved == null) throw new ArgumentNullException("taskRemoved");
    
                    var taskRemoveFilter = Builders<ReportEntity>.Filter.Eq(r => r.OrderId, taskRemoved.OrderId);
                    var taskRemoveUpdate = Builders<ReportEntity>.Update.PullFilter(r => r.ServiceTasks, t => t.Id == taskRemoved.Id);

                    await _dbContext.Reports.UpdateOneAsync(taskRemoveFilter, taskRemoveUpdate);
                    break;
                
                case RoutingKey.PartRemoved:
                    var partRemoved = JsonSerializer.Deserialize<PartRemovedMessage>(message);
                    if (partRemoved == null) throw new ArgumentNullException("partRemoved");

                    var partRemoveFilter = Builders<ReportEntity>.Filter.Eq(r => r.OrderId, partRemoved.OrderId);
                    var partRemoveUpdate = Builders<ReportEntity>.Update.PullFilter(
                        r => r.ServiceParts, p => p.Id == partRemoved.Id
                    );

                    await _dbContext.Reports.UpdateOneAsync(partRemoveFilter, partRemoveUpdate);
                    break;
                
                case RoutingKey.OrderUpdated:
                    var orderUpdated = JsonSerializer.Deserialize<OrderUpdatedMessage>(message);
                    if (orderUpdated == null) throw new ArgumentNullException("orderUpdated");

                    var orderUpdateFilter = Builders<ReportEntity>.Filter.Eq(r => r.OrderId, orderUpdated.Id);
                    var orderUpdateDefinition = Builders<ReportEntity>.Update
                        .Set(r => r.Vehicle, orderUpdated.Vehicle)
                        .Set(r => r.User, orderUpdated.User)
                        .Set(r => r.Status, orderUpdated.Status);

                    await _dbContext.Reports.UpdateOneAsync(orderUpdateFilter, orderUpdateDefinition);
                    break;
                
                case RoutingKey.CommentUpdated:
                    var commentUpdated = JsonSerializer.Deserialize<CommentUpdatedMessage>(message);
                    if (commentUpdated == null) throw new ArgumentNullException("commentUpdated");

                    var commentUpdateFilter = Builders<ReportEntity>.Filter.And(
                        Builders<ReportEntity>.Filter.Eq(r => r.OrderId, commentUpdated.OrderId),
                        Builders<ReportEntity>.Filter.ElemMatch(r => r.ServiceComments, c => c.Id == commentUpdated.Id)
                    );

                    var commUpdate = Builders<ReportEntity>.Update
                        .Set("ServiceComments.$.Content", commentUpdated.Content);

                    await _dbContext.Reports.UpdateOneAsync(commentUpdateFilter, commUpdate);
                    break;
                
                case RoutingKey.PartUpdated:
                    var partUpdated = JsonSerializer.Deserialize<PartUpdatedMessage>(message);
                    if (partUpdated == null) throw new ArgumentNullException("partUpdated");

                    var partUpdateFilter = Builders<ReportEntity>.Filter.And(
                        Builders<ReportEntity>.Filter.Eq(r => r.OrderId, partUpdated.OrderId),
                        Builders<ReportEntity>.Filter.ElemMatch(r => r.ServiceParts, p => p.Id == partUpdated.Id)
                    );

                    var partUpdate = Builders<ReportEntity>.Update
                        .Set("ServiceParts.$.VehiclePart", partUpdated.VehiclePart)
                        .Set("ServiceParts.$.Quantity", partUpdated.Quantity);

                    await _dbContext.Reports.UpdateOneAsync(partUpdateFilter, partUpdate);
                    break;
                
                case RoutingKey.TaskUpdated:
                    var taskUpdated = JsonSerializer.Deserialize<TaskUpdatedMessage>(message);
                    if (taskUpdated == null) throw new ArgumentNullException("taskUpdated");

                    var taskUpdateFilter = Builders<ReportEntity>.Filter.And(
                        Builders<ReportEntity>.Filter.Eq(r => r.OrderId, taskUpdated.OrderId),
                        Builders<ReportEntity>.Filter.ElemMatch(r => r.ServiceTasks, t => t.Id == taskUpdated.Id)
                    );

                    var tUpdate = Builders<ReportEntity>.Update
                        .Set("ServiceTasks.$.Description", taskUpdated.Description)
                        .Set("ServiceTasks.$.LaborCost", taskUpdated.LaborCost);

                    await _dbContext.Reports.UpdateOneAsync(taskUpdateFilter, tUpdate);
                    break;
                
                default:
                    Console.WriteLine("⚠️ Unhandled routing key.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("⚠️ Unknown routing key received.");
        }
    }
}

public class UserRoleAddedMessage {
    public int UserId { get; set; }
    public RoleEntity Role { get; set; }
}

public class UserRoleRemovedMessage {
    public int UserId { get; set; }
    public RoleEntity Role { get; set; }
}

public class ReportCreatedMessage {
    public int Id { get; set; }
    public VehicleEntity Vehicle { get; set; }
    public string Status { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TaskCreatedMessage {
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Description { get; set; }
    public Decimal LaborCost { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PartAddedMessage {
    public int Id { get; set; }
    public int OrderId { get; set; }
    public VehiclePartEntity VehiclePart { get; set; }
    public int Quantity { get; set; }
}

public class CommentAddedMessage {
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class StatusChangedMessage {
    public int OrderId { get; set; }
    public string Status { get; set; }
}

public class OrderDeletedMessage {
    public int Id { get; set; }
}

public class TaskRemovedMessage {
    public int Id { get; set; }
    public int OrderId { get; set; }
}

public class CommentRemovedMessage {
    public int Id { get; set; }
    public int OrderId { get; set; }
}

public class PartRemovedMessage {
    public int Id { get; set; }
    public int OrderId { get; set; }
}

public class OrderUpdatedMessage {
    public int Id { get; set; }
    public VehicleEntity Vehicle { get; set; }
    public UserEntity User { get; set; }
    public string Status { get; set; }
}

public class CommentUpdatedMessage {
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Content { get; set; }
}

public class PartUpdatedMessage {
    public int Id { get; set; }
    public int OrderId { get; set; }
    public VehiclePartEntity VehiclePart { get; set; }
    public int Quantity { get; set; }
}

public class TaskUpdatedMessage {
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Description { get; set; }
    public decimal LaborCost { get; set; }
}

public enum RoutingKey {
    // For users
    UserCreated,
    UserRoleAdded,
    UserRoleRemoved,
    
    // Create / Add
    OrderCreated,
    TaskAdded,
    PartAdded,
    CommentAdded,
    StatusChanged,
    
    // Delete
    OrderDeleted,
    CommentRemoved,
    PartRemoved,
    TaskRemoved,
    
    // Update
    OrderUpdated,
    CommentUpdated,
    PartUpdated,
    TaskUpdated,
}

public static class RoutingKeyExtensions
{
    private static readonly Dictionary<RoutingKey, string> _toString = new()
    {
        { RoutingKey.UserCreated, "workshop.service.user.created"},
        { RoutingKey.UserRoleAdded, "workshop.service.user.role_added"},
        { RoutingKey.UserRoleRemoved, "workshop.service.user.role_removed"},
        
        { RoutingKey.OrderCreated, "workshop.service.order.created" },
        { RoutingKey.TaskAdded, "workshop.service.task.added" },
        { RoutingKey.PartAdded, "workshop.service.part.added" },
        { RoutingKey.CommentAdded, "workshop.service.comment.added" },
        { RoutingKey.StatusChanged, "workshop.service.status.changed" },
        
        { RoutingKey.OrderDeleted, "workshop.service.order.deleted" },
        { RoutingKey.CommentRemoved, "workshop.service.comment.removed" },
        { RoutingKey.PartRemoved, "workshop.service.part.removed" },
        { RoutingKey.TaskRemoved, "workshop.service.task.removed" },

        { RoutingKey.OrderUpdated, "workshop.service.order.updated" },
        { RoutingKey.CommentUpdated, "workshop.service.comment.updated" },
        { RoutingKey.PartUpdated, "workshop.service.part.updated" },
        { RoutingKey.TaskUpdated, "workshop.service.task.updated" },
    };

    private static readonly Dictionary<string, RoutingKey> _fromString = _toString
        .ToDictionary(kv => kv.Value, kv => kv.Key);

    public static string ToRoutingKeyString(this RoutingKey key)
        => _toString.TryGetValue(key, out var val) ? val : key.ToString();

    public static bool TryParse(string str, out RoutingKey key)
        => _fromString.TryGetValue(str, out key);
}