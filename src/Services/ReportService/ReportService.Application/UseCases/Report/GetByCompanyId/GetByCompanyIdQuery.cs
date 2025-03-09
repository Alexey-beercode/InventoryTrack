using MediatR;
using ReportService.Application.DTOs.Response.Report;

namespace ReportService.Application.UseCases.Report.GetByCompanyId;

public class GetByCompanyIdQuery : IRequest<IEnumerable<ReportResponseDto>>
{
    public Guid CompanyId { get; set; }
}