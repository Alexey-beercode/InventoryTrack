using AutoMapper;
using MediatR;
using ReportService.Application.DTOs.Response.Report;
using ReportService.Application.Exceptions;
using ReportService.Domain.Interfaces.Repositories;
using ReportService.Domain.Interfaces.Services;

namespace ReportService.Application.UseCases.Report.GetById;

public class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, ReportExportResponseDto>
{
    private readonly IMapper _mapper;
    private readonly IReportRepository _reportRepository;
    private readonly IExcelExportService _excelExportService;

    public GetReportByIdQueryHandler(IMapper mapper, IReportRepository reportRepository, IExcelExportService excelExportService)
    {
        _mapper = mapper;
        _reportRepository = reportRepository;
        _excelExportService = excelExportService;
    }

    public async Task<ReportExportResponseDto> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
    {
        // Получаем отчет из репозитория
        var report = await _reportRepository.GetByIdAsync(request.Id);

        if (report == null)
        {
            throw new EntityNotFoundException($"Report with Id {request.Id} was not found.");
        }

        var exportReport =await _excelExportService.GenerateExcelReportAsync(report);
        return new ReportExportResponseDto(){Content = exportReport, Name = report.Name};
    }
}