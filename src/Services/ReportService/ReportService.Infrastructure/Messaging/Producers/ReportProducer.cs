using System.Net;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using ReportService.Domain.Enums;

namespace ReportService.Infrastructure.Messaging.Producers;

public class ReportProducer
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ReportProducer> _logger;

    public ReportProducer(IHttpClientFactory httpClientFactory, ILogger<ReportProducer> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<BsonDocument> RequestReportDataAsync(Guid companyId, ReportType reportType, DateSelect dateSelect)
    {
        var clientName = reportType switch
        {
            ReportType.StockState => "InventoryService",
            ReportType.Items => "InventoryService",
            ReportType.Movements => "MovementService",
            ReportType.WriteOffs => "WriteOffService",
            _ => throw new ArgumentOutOfRangeException(nameof(reportType), "Unsupported report type")
        };

        var client = _httpClientFactory.CreateClient(clientName);

        var endpoint = reportType switch
        {
            ReportType.StockState => $"report-data?reportType=warehouses&companyId={companyId}&dateSelect={(int)dateSelect}",
            ReportType.Items => $"report-data?reportType=items&dateSelect={(int)dateSelect}",
            ReportType.Movements => $"report-data?reportType=movements&dateSelect={(int)dateSelect}",
            ReportType.WriteOffs => $"report-data?reportType=writeOffs&companyId={companyId}&dateSelect={(int)dateSelect}",
            _ => throw new ArgumentOutOfRangeException(nameof(reportType), "Unsupported report type")
        };
        _logger.LogInformation("Requesting data from endpoint: {Endpoint}", endpoint);

        var response = await client.GetAsync(endpoint);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("No data found for endpoint: {Endpoint}", endpoint);
            return new BsonDocument { { "Message", "No data available" } };
        }
        
        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            _logger.LogError("Request to {Endpoint} failed with status {StatusCode}: {Details}", endpoint, response.StatusCode, errorDetails);
            throw new HttpRequestException($"Error response: {response.StatusCode}, Details: {errorDetails}");
        }

        // Парсинг через MongoDB.Bson
        var jsonString = await response.Content.ReadAsStringAsync();
        var reportData = BsonDocument.Parse(jsonString);

        if (reportType == ReportType.WriteOffs)
        {
            return await EnrichWriteOffsReportAsync(reportData);
        }

        return reportData;
    }

    private async Task<BsonDocument> EnrichWriteOffsReportAsync(BsonDocument reportData)
    {
        var authClient = _httpClientFactory.CreateClient("AuthService");
        var inventoryClient = _httpClientFactory.CreateClient("InventoryService");

        var writeOffs = reportData["WriteOffs"].AsBsonArray;

        foreach (var writeOff in writeOffs.Cast<BsonDocument>())
        {
            // Получение данных пользователя
            var userId = writeOff["ApprovedByUser"].AsString;
            var userResponse = await authClient.GetAsync($"users/{userId}");
            if (userResponse.IsSuccessStatusCode)
            {
                var userJson = await userResponse.Content.ReadAsStringAsync();
                var user = BsonDocument.Parse(userJson);
                writeOff["ApprovedByUser"] = $"{user["FirstName"]} {user["LastName"]}";
            }

            // Получение данных товара
            var itemId = writeOff["ItemId"].AsString;
            var itemResponse = await inventoryClient.GetAsync($"items/{itemId}");
            if (itemResponse.IsSuccessStatusCode)
            {
                var itemJson = await itemResponse.Content.ReadAsStringAsync();
                var item = BsonDocument.Parse(itemJson);
                writeOff["ItemName"] = item["Name"].AsString;
            }

            // Получение данных склада
            var warehouseId = writeOff["WarehouseId"].AsString;
            var warehouseResponse = await inventoryClient.GetAsync($"warehouses/{warehouseId}");
            if (warehouseResponse.IsSuccessStatusCode)
            {
                var warehouseJson = await warehouseResponse.Content.ReadAsStringAsync();
                var warehouse = BsonDocument.Parse(warehouseJson);
                writeOff["WarehouseName"] = warehouse["Name"].AsString;
            }
        }

        return reportData;
    }
}