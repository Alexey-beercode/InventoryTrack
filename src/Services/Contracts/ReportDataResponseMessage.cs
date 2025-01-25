using MongoDB.Bson;

namespace ReportService.Messaging.Messages
{
    public class ReportDataResponseMessage
    {
        public Guid ReportRequestId { get; set; }
        public BsonDocument Data { get; set; }
    }
}