using MediatR;
using ReportService.Domain.Enums;

namespace ReportService.Application.UseCases.Report.Create;

public class CreateCommand : IRequest
{
    public ReportType ReportType { get; set; }
    public DateSelect DateSelect { get; set; }
    public Guid CompanyId { get; set; }
}