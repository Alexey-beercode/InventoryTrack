using AutoMapper;
using MediatR;
using ReportService.Application.DTOs.Response.Report;
using ReportService.Domain.Interfaces.Repositories;

namespace ReportService.Application.UseCases.Report.GetByCompanyId;

public class GetByCompanyIdQueryHandler : IRequestHandler<GetByCompanyIdQuery,IEnumerable<ReportResponseDto>>
{
    private readonly IReportRepository _reportRepository;
    private readonly IMapper _mapper;

    public GetByCompanyIdQueryHandler(IReportRepository reportRepository, IMapper mapper)
    {
        _reportRepository = reportRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReportResponseDto>> Handle(GetByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        var reports = await _reportRepository.GetByCompanyIdAsync(request.CompanyId, cancellationToken);
        return _mapper.Map<IEnumerable<ReportResponseDto>>(reports);
    }
}