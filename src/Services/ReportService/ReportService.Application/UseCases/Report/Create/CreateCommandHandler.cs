using MediatR;
using ReportService.Domain.Interfaces.Repositories;
using ReportService.Infrastructure.Messaging.Producers;

namespace ReportService.Application.UseCases.Report.Create;

public class CreateCommandHandler : IRequestHandler<CreateCommand>
{
    private readonly ReportProducer _reportProducer;
    private readonly IReportRepository _reportRepository;

    public CreateCommandHandler(ReportProducer reportProducer, IReportRepository reportRepository)
    {
        _reportProducer = reportProducer;
        _reportRepository = reportRepository;
    }

    public async Task Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        // Запрашиваем данные отчета
        var reportData = await _reportProducer.RequestReportDataAsync(request.CompanyId, request.ReportType, request.DateSelect);

        // Создаем новый отчет
        var report = new Domain.Entities.Report
        {
            Name = $"{request.ReportType}_{DateTime.UtcNow:yyyyMMddHHmmss}",
            Data = reportData,
            ReportType = request.ReportType,
            DateSelect = request.DateSelect,
            CreatedAt = DateTime.UtcNow
        };

        // Сохраняем отчет в базе данных
        await _reportRepository.CreateAsync(report);
    }
}