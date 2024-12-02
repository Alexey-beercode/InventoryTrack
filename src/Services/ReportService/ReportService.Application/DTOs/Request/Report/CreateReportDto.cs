using ReportService.Domain.Enums;

namespace ReportService.Application.DTOs.Request.Report;

public class CreateReportDto
{
    public ReportType ReportType { get; set; }
    public DateSelect DateSelect { get; set; }
}