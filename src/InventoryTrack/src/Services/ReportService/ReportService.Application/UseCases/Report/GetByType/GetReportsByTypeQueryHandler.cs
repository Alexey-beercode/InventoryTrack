using AutoMapper;
using MediatR;
using ReportService.Application.DTOs.Response.Report;
using ReportService.Domain.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReportService.Application.UseCases.Report.GetByType;

public class GetReportsByTypeQueryHandler : IRequestHandler<GetReportsByTypeQuery, IEnumerable<ReportResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IReportRepository _reportRepository;

    public GetReportsByTypeQueryHandler(IMapper mapper, IReportRepository reportRepository)
    {
        _mapper = mapper;
        _reportRepository = reportRepository;
    }

    public async Task<IEnumerable<ReportResponseDto>> Handle(GetReportsByTypeQuery request, CancellationToken cancellationToken)
    {
        // Получаем отчеты из репозитория
        var reports = await _reportRepository.GetByTypeAsync(request.ReportType, cancellationToken);

        // Маппим отчеты в DTO
        return _mapper.Map<IEnumerable<ReportResponseDto>>(reports);
    }
}