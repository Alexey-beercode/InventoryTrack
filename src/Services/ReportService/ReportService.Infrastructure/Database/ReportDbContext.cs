using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ReportService.Domain.Entities;
using ReportService.Infrastructure.Database.Configuration;

namespace ReportService.Infrastructure.Database;

public class ReportDbContext
{
    private readonly IMongoDatabase _database;

    public ReportDbContext(IOptions<NotificationDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }

    public IMongoCollection<Report> Notifications => _database.GetCollection<Report>("reports");
}