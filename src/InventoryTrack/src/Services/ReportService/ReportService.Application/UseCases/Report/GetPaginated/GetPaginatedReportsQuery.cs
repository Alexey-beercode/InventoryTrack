using MediatR;
using ReportService.Application.DTOs.Response.Report;

namespace ReportService.Application.UseCases.Report.GetPaginated;

public class GetPaginatedReportsQuery : IRequest<IEnumerable<ReportResponseDto>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}