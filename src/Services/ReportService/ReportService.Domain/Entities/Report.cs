using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ReportService.Domain.Common;
using ReportService.Domain.Enums;

namespace ReportService.Domain.Entities;

public class Report : BaseEntity
{
    [BsonElement("data")]
    public string Name { get; set; }
    public BsonDocument Data { get; set; }
    public ReportType ReportType { get; set; }
    public DateSelect DateSelect { get; set; }
    public DateTime CreatedAt { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid CompanyId{get;set;}
}