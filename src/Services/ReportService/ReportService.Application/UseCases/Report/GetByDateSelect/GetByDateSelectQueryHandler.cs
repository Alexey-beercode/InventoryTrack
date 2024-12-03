using AutoMapper;
using MediatR;
using ReportService.Application.DTOs.Response.Report;
using ReportService.Domain.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReportService.Application.UseCases.Report.GetByDateSelect;

public class GetByDateSelectQueryHandler : IRequestHandler<GetByDateSelectQuery, IEnumerable<ReportResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IReportRepository _reportRepository;

    public GetByDateSelectQueryHandler(IMapper mapper, IReportRepository reportRepository)
    {
        _mapper = mapper;
        _reportRepository = reportRepository;
    }

    public async Task<IEnumerable<ReportResponseDto>> Handle(GetByDateSelectQuery request, CancellationToken cancellationToken)
    {
        var reports = await _reportRepository.GetByDateSelectAsync(request.DateSelect, cancellationToken);
        
        return _mapper.Map<IEnumerable<ReportResponseDto>>(reports);
    }
}