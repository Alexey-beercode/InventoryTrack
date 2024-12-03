using MediatR;
using ReportService.Application.DTOs.Response.Report;

namespace ReportService.Application.UseCases.Report.GetById;

public class GetReportByIdQuery : IRequest<ReportExportResponseDto>
{
    public Guid Id { get; set; }
}