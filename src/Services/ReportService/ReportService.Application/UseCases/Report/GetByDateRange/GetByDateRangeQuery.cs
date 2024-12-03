using MediatR;
using ReportService.Application.DTOs.Response.Report;

namespace ReportService.Application.UseCases.Report.GetByDateRange;

public class GetByDateRangeQuery : IRequest<IEnumerable<ReportResponseDto>>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}