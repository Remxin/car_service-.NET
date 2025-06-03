using MongoDB.Bson;
using MongoDB.Driver;
using ReportService.Entities;

namespace ReportService.Data;

public class MongoDbContext {
    private readonly IMongoDatabase _database;

    public MongoDbContext(String connectionString) {
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        var client = new MongoClient(settings);
        _database = client.GetDatabase("reportDb");
        try {
            var ping = _database.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            Console.WriteLine("✅ MongoDB ping successful: " + ping.ToJson());
        }
        catch (Exception ex) {
            Console.WriteLine("❌ MongoDB connection failed:");
            Console.WriteLine(ex);
        }
    }
    
    public void TestConnection() {
        var command = new BsonDocument("ping", 1);
        _database.RunCommand<BsonDocument>(command);
    }

    public IMongoCollection<ReportEntity> Reports => _database.GetCollection<ReportEntity>("reports");
    public IMongoCollection<UserEntity> Users => _database.GetCollection<UserEntity>("users");
}