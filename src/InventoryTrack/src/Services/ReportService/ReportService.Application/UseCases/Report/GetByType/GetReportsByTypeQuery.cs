using MediatR;
using ReportService.Application.DTOs.Response.Report;
using ReportService.Domain.Enums;

namespace ReportService.Application.UseCases.Report.GetByType;

public class GetReportsByTypeQuery : IRequest<IEnumerable<ReportResponseDto>>
{
    public ReportType ReportType { get; set; }
}