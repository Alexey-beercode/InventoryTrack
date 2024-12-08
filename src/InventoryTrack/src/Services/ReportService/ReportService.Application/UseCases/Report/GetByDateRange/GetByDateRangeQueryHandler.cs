using AutoMapper;
using MediatR;
using ReportService.Application.DTOs.Response.Report;
using ReportService.Domain.Interfaces.Repositories;

namespace ReportService.Application.UseCases.Report.GetByDateRange;

public class GetByDateRangeQueryHandler : IRequestHandler<GetByDateRangeQuery,IEnumerable<ReportResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IReportRepository _reportRepository;

    public GetByDateRangeQueryHandler(IMapper mapper, IReportRepository reportRepository)
    {
        _mapper = mapper;
        _reportRepository = reportRepository;
    }

    public async Task<IEnumerable<ReportResponseDto>> Handle(GetByDateRangeQuery request, CancellationToken cancellationToken)
    {
        var reports = new List<Domain.Entities.Report>();
        foreach (var report in await _reportRepository.GetByDateRangeAsync(request.StartDate, request.EndDate,
                     cancellationToken))
        {
            reports.Add(report);
        }

        return _mapper.Map<IEnumerable<ReportResponseDto>>(reports);
    }
}