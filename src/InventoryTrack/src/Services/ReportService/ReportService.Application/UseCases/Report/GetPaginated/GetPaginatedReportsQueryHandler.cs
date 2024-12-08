using AutoMapper;
using MediatR;
using ReportService.Application.DTOs.Response.Report;
using ReportService.Domain.Interfaces.Repositories;

namespace ReportService.Application.UseCases.Report.GetPaginated;

public class GetPaginatedReportsQueryHandler : IRequestHandler<GetPaginatedReportsQuery, IEnumerable<ReportResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IReportRepository _reportRepository;

    public GetPaginatedReportsQueryHandler(IMapper mapper, IReportRepository reportRepository)
    {
        _mapper = mapper;
        _reportRepository = reportRepository;
    }

    public async Task<IEnumerable<ReportResponseDto>> Handle(GetPaginatedReportsQuery request, CancellationToken cancellationToken)
    {
        // Получаем отчеты из репозитория
        var reports = await _reportRepository.GetPaginatedAsync(request.PageNumber, request.PageSize, cancellationToken);

        // Маппим отчеты в DTO
        return _mapper.Map<IEnumerable<ReportResponseDto>>(reports);
    }
}