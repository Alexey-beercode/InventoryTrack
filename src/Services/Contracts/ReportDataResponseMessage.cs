using MongoDB.Bson;

public class ReportDataResponseMessage
{
    public Guid ReportRequestId { get; set; }
    public BsonDocument Data { get; set; }
}