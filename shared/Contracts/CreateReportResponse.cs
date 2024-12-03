using MongoDB.Bson;

namespace ReportService.Infrastructure.Messaging.Messages
{
    public class CreateReportResponse
    {
        public BsonDocument Data { get; set; }
    }
}