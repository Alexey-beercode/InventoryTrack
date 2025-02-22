using MongoDB.Bson;

namespace Contracts
{
    public class ReportDataResponseMessage
    {
        public Guid ReportRequestId { get; set; }
        public BsonDocument Data { get; set; }
    }
}