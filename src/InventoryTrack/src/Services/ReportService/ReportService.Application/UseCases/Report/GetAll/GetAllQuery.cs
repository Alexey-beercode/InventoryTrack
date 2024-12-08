using MediatR;
using ReportService.Application.DTOs.Response.Report;

namespace ReportService.Application.UseCases.Report.GetAll;

public class GetAllQuery : IRequest<IEnumerable<ReportResponseDto>>
{
    
}