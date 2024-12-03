using AutoMapper;
using MediatR;
using ReportService.Application.DTOs.Response.Report;
using ReportService.Domain.Interfaces.Repositories;

namespace ReportService.Application.UseCases.Report.GetAll;

public class GetAllQueryHandler : IRequestHandler<GetAllQuery,IEnumerable<ReportResponseDto>>
{
    private readonly IReportRepository _reportRepository;
    private readonly IMapper _mapper;

    public GetAllQueryHandler(IReportRepository reportRepository, IMapper mapper)
    {
        _reportRepository = reportRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReportResponseDto>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        var reports = new List<Domain.Entities.Report>();
        await foreach (var report in _reportRepository.GetAllAsync())
        {
            reports.Add(report);
        }

        return _mapper.Map<IEnumerable<ReportResponseDto>>(reports);
    }
}