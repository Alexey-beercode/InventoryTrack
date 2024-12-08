using MediatR;
using ReportService.Application.DTOs.Response.Report;
using ReportService.Domain.Enums;

namespace ReportService.Application.UseCases.Report.GetByDateSelect;

public class GetByDateSelectQuery : IRequest<IEnumerable<ReportResponseDto>>
{
    public DateSelect DateSelect { get; set; }
}