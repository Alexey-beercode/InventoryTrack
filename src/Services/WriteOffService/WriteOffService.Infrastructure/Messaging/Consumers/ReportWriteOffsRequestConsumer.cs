using MassTransit;
using WriteOffService.Application.DTOs.Request.WriteOffRequest;
using WriteOffService.Application.DTOs.Response.WriteOffRequest;
using WriteOffService.Application.Interfaces.Services;
using MongoDB.Bson;

namespace WriteOffService.Infrastructure.Messaging.Consumers;

public class ReportWriteOffsRequestConsumer : IConsumer<GetReportDataMessage>
{
    private readonly IWriteOffRequestService _writeOffRequestService;

    public ReportWriteOffsRequestConsumer(IWriteOffRequestService writeOffRequestService)
    {
        _writeOffRequestService = writeOffRequestService;
    }

    public async Task Consume(ConsumeContext<GetReportDataMessage> context)
    {
        var message = context.Message;
        if (message.ReportType == "writeOffs")
        {
            // Определяем временные рамки
            var startDate = new DateTime();
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

            var endDate = DateTime.UtcNow;

            // Получаем записи списаний
            var writeOffs = await _writeOffRequestService.GetFilteredPagedRequestsAsync(new WriteOffRequestFilterDto
            {
                CompanyId = message.CompanyId,
                IsPaginated = false,
                StartDate = startDate,
                EndDate = endDate
            });

            // Формируем BSON-документ для отчета
            var bsonData = new BsonDocument
            {
                {
                    "WriteOffs", new BsonArray(writeOffs.Select(writeOff => new BsonDocument
                    {
                        { "ItemId", writeOff.ItemId.ToString() },
                        { "WarehouseId", writeOff.WarehouseId.ToString() },
                        { "Quantity", writeOff.Quantity },
                        { "Status", writeOff.Status.Name },
                        { "RequestDate", writeOff.RequestDate },
                        { "Reason", writeOff.Reason.Reason },
                        {"ApprovedByUser",writeOff.ApprovedByUserId.ToString()}
                    }))
                }
            };

            // Формируем ответ
            var response = new ReportDataResponseMessage
            {
                ReportRequestId = message.ReportRequestId,
                Data = bsonData
            };

            // Отправляем ответ
            await context.RespondAsync(response);
        }
    }
}
