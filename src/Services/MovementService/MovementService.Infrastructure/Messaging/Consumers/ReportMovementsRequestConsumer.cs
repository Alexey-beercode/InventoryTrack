using MassTransit;
using MongoDB.Bson;
using MovementService.Domain.Interfaces.UnitOfWork;

namespace MovementService.Infrastructure.Messaging.Consumers;

public class ReportMovementsRequestConsumer : IConsumer<GetReportDataMessage>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportMovementsRequestConsumer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<GetReportDataMessage> context)
    {
        var message = context.Message;

        if (message.ReportType == "movements")
        {
            // Получаем данные по перемещениям за указанный период
            var startDate = DateTime.UtcNow;
            var endDate = DateTime.UtcNow;

            switch (message.DateSelect)
            {
                case "Day":
                    startDate = DateTime.UtcNow.AddDays(-1);
                    break;
                case "Week":
                    startDate = DateTime.UtcNow.AddDays(-7);
                    break;
                case "Month":
                    startDate = DateTime.UtcNow.AddMonths(-1);
                    break;
                case "Year":
                    startDate = DateTime.UtcNow.AddYears(-1);
                    break;
            }

            var movements = await _unitOfWork.MovementRequests.GetByDateRangeAsync(startDate, endDate);
            var bsonData = new BsonDocument
            {
                {
                    "Movements", new BsonArray(movements.Select(m => new BsonDocument
                    {
                        { "ItemId", m.ItemId.ToString() },
                        { "SourceWarehouseId", m.SourceWarehouseId.ToString() },
                        { "DestinationWarehouseId", m.DestinationWarehouseId.ToString() },
                        { "Quantity", m.Quantity },
                        { "Status", m.Status.ToString() },
                        { "RequestDate", m.RequestDate },
                        { "ApprovedByUserId", m.ApprovedByUserId.ToString() }
                    }))
                }
            };

            await context.RespondAsync(new ReportDataResponseMessage
            {
                ReportRequestId = message.ReportRequestId,
                Data = bsonData
            });
        }
    }
}