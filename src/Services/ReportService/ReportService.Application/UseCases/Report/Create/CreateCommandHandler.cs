using MediatR;
using MongoDB.Bson;
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
        var jsonReportData = await _reportProducer.RequestReportDataAsync(request.CompanyId, request.ReportType, request.DateSelect, request.WarehouseId);

        // Преобразуем JSON в BsonDocument перед сохранением в БД
        var bsonReportData = BsonDocument.Parse(jsonReportData);

        // Создаем новый отчет
        var report = new Domain.Entities.Report
        {
            Name = $"{request.ReportType}_{DateTime.UtcNow:yyyyMMddHHmmss}",
            Data = bsonReportData,
            ReportType = request.ReportType,
            DateSelect = request.DateSelect,
            CreatedAt = DateTime.UtcNow,
            CompanyId = request.CompanyId
        };

        // Сохраняем отчет в базе данных
        await _reportRepository.CreateAsync(report);
    }
}