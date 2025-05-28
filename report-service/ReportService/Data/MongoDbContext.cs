using MongoDB.Driver;
using ReportService.Entities;

namespace ReportService.Data;

public class MongoDbContext {
    private readonly IMongoDatabase _database;

    public MongoDbContext(String connectionString) {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("reportDb");
    }

    public IMongoCollection<ReportEntity> Reports => _database.GetCollection<ReportEntity>("reports");
    public IMongoCollection<UserEntity> Users => _database.GetCollection<UserEntity>("users");
}