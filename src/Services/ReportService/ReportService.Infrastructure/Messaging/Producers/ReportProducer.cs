using MassTransit;
using MongoDB.Bson;
using ReportService.Domain.Enums;

namespace ReportService.Infrastructure.Messaging.Producers;

public class ReportProducer
{
    private readonly IRequestClient<GetReportDataMessage> _reportDataRequestClient;
    private readonly IRequestClient<GetUserMessage> _userRequestClient;
    private readonly IRequestClient<GetItemMessage> _itemRequestClient;
    private readonly IRequestClient<GetWarehouseMessage> _warehouseRequestClient;

    public ReportProducer(
        IRequestClient<GetReportDataMessage> reportDataRequestClient,
        IRequestClient<GetUserMessage> userRequestClient,
        IRequestClient<GetItemMessage> itemRequestClient,
        IRequestClient<GetWarehouseMessage> warehouseRequestClient)
    {
        _reportDataRequestClient = reportDataRequestClient;
        _userRequestClient = userRequestClient;
        _itemRequestClient = itemRequestClient;
        _warehouseRequestClient = warehouseRequestClient;
    }

    public async Task<BsonDocument> RequestReportDataAsync(Guid companyId, ReportType reportType, DateSelect dateSelect)
    {
        var reportMessage = new GetReportDataMessage
        {
            CompanyId = companyId,
            ReportRequestId = Guid.NewGuid(),
            ReportType = reportType.ToString(),
            DateSelect = dateSelect.ToString()
        };

        var reportResponse = await _reportDataRequestClient.GetResponse<ReportDataResponseMessage>(reportMessage);

        if (reportType == ReportType.WriteOffs)
        {
            return await EnrichWriteOffsReportAsync(reportResponse.Message.Data);
        }

        return reportResponse.Message.Data;
    }

    private async Task<BsonDocument> EnrichWriteOffsReportAsync(BsonDocument reportData)
    {
        var writeOffs = reportData["WriteOffs"].AsBsonArray;

        foreach (var writeOff in writeOffs.Cast<BsonDocument>())
        {
            // Получение данных пользователя
            var userId = Guid.Parse(writeOff["ApprovedByUserId"].AsString);
            var userResponse = await _userRequestClient.GetResponse<UserResponseMessage>(new GetUserMessage { UserId = userId });
            writeOff["ApprovedByUser"] = $"{userResponse.Message.FirstName} {userResponse.Message.LastName}";

            // Получение данных товара
            var itemId = Guid.Parse(writeOff["ItemId"].AsString);
            var itemResponse = await _itemRequestClient.GetResponse<ItemResponseMessage>(new GetItemMessage { Id = itemId });
            writeOff["ItemName"] = itemResponse.Message.Name;

            // Получение данных склада
            var warehouseId = Guid.Parse(writeOff["WarehouseId"].AsString);
            var warehouseResponse = await _warehouseRequestClient.GetResponse<WarehouseResponseMessage>(new GetWarehouseMessage { Id = warehouseId });
            writeOff["WarehouseName"] = warehouseResponse.Message.Name;
        }

        return reportData;
    }
}