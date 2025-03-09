using ReportService.Domain.Enums;

namespace ReportService.Application.DTOs.Response.Report;

public class ReportResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ReportType ReportType { get; set; }
    public DateSelect DateSelect { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CompanyId { get; set; }
}