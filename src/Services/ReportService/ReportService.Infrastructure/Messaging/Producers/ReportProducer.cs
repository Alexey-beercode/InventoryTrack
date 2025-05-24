using System.Net;
using System.Text.Json;
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

    public async Task<string> RequestReportDataAsync(Guid companyId, ReportType reportType, DateSelect dateSelect, Guid? warehouseId = null)
    {
        var clientName = GetClientName(reportType);
        var endpoint = GetEndpoint(reportType, companyId, dateSelect, warehouseId);
        _logger.LogInformation("🔍 Запрос данных с эндпоинта: {Endpoint}", endpoint);

        var client = _httpClientFactory.CreateClient(clientName);
        var response = await client.GetAsync(endpoint);
      
        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            _logger.LogError("❌ Ошибка при запросе {Endpoint}, статус {StatusCode}: {Details}", endpoint, response.StatusCode, errorDetails);
            throw new HttpRequestException($"Ошибка: {response.StatusCode}, Детали: {errorDetails}");
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("📦 Исходные данные: {JsonString}", jsonString);
        
        // Если это Movements или WriteOffs - обогащаем данные
        if (reportType == ReportType.Movements || reportType == ReportType.WriteOffs)
        {
            var stockStateData = await RequestStockStateDataAsync(companyId, dateSelect);
            return reportType == ReportType.Movements
                ? await EnrichMovementsReport(jsonString, stockStateData)
                : await EnrichWriteOffsReport(jsonString, stockStateData);
        }
        
        _logger.LogInformation("📦 Финальные данные: {JsonString}", jsonString);
        return jsonString;
    }

    private async Task<string> RequestStockStateDataAsync(Guid companyId, DateSelect dateSelect)
    {
        var client = _httpClientFactory.CreateClient("InventoryService");
        var endpoint = $"report-data?reportType=warehouses&companyId={companyId}&dateSelect={(int)dateSelect}";
        _logger.LogInformation("🔍 Запрос общего состояния склада с эндпоинта: {Endpoint}", endpoint);

        var response = await client.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            _logger.LogError("❌ Ошибка при запросе состояния склада: {StatusCode}, {Details}", response.StatusCode, errorDetails);
            throw new HttpRequestException($"Ошибка: {response.StatusCode}, Детали: {errorDetails}");
        }

        var stockStateJson = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("📦 Полученный stockState: {StockState}", stockStateJson);
        return stockStateJson;
    }

    private static string GetClientName(ReportType reportType) => reportType switch
    {
        ReportType.StockState or ReportType.Items => "InventoryService",
        ReportType.Movements => "MovementService",
        ReportType.WriteOffs => "WriteOffService",
        _ => throw new ArgumentOutOfRangeException(nameof(reportType), "Unsupported report type")
    };

    private static string GetEndpoint(ReportType reportType, Guid companyId, DateSelect dateSelect, Guid? warehouseId) =>
        reportType switch
        {
            ReportType.StockState => warehouseId.HasValue
                ? $"items/by-warehouse/{warehouseId}"
                : $"report-data?reportType=warehouses&companyId={companyId}&dateSelect={(int)dateSelect}",

            ReportType.Items => $"report-data?reportType=items&dateSelect={(int)dateSelect}",

            ReportType.Movements => warehouseId.HasValue
                ? $"report-data/by-warehouse?warehouseId={warehouseId}&dateSelect={(int)dateSelect}"
                : $"report-data?reportType=movements&dateSelect={(int)dateSelect}",

            ReportType.WriteOffs => $"report-data?reportType=writeOffs&companyId={companyId}&dateSelect={(int)dateSelect}",

            _ => throw new ArgumentOutOfRangeException(nameof(reportType), $"Unsupported report type: {reportType}")
        };

    private async Task<string> EnrichMovementsReport(string reportDataJson, string stockStateJson)
    {
        using var reportDoc = JsonDocument.Parse(reportDataJson);
        using var stockDoc = JsonDocument.Parse(stockStateJson);

        // ✅ Проверяем правильную структуру ответа от InventoryService
        if (!stockDoc.RootElement.TryGetProperty("warehouses", out var warehousesElement) || warehousesElement.ValueKind != JsonValueKind.Array)
        {
            _logger.LogWarning("⚠ Ошибка: в stockStateJson отсутствует массив 'warehouses' (с маленькой буквы)");
            return reportDataJson;
        }

        // Создаем словарь складов и товаров
        var warehousesDict = new Dictionary<string, JsonElement>();
        var itemsDict = new Dictionary<string, (string itemName, string warehouseName)>();

        foreach (var warehouse in warehousesElement.EnumerateArray())
        {
            if (!warehouse.TryGetProperty("id", out var idProp) || idProp.ValueKind != JsonValueKind.String)
                continue;

            string warehouseId = idProp.GetString()!;
            warehousesDict[warehouseId] = warehouse;

            string warehouseName = warehouse.TryGetProperty("name", out var nameProp) && nameProp.ValueKind == JsonValueKind.String
                ? nameProp.GetString()!
                : "Неизвестный склад";

            // Получаем товары из этого склада
            if (warehouse.TryGetProperty("items", out var itemsElement) && itemsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in itemsElement.EnumerateArray())
                {
                    if (item.TryGetProperty("id", out var itemIdProp) && itemIdProp.ValueKind == JsonValueKind.String)
                    {
                        string itemId = itemIdProp.GetString()!;
                        string itemName = item.TryGetProperty("name", out var itemNameProp) && itemNameProp.ValueKind == JsonValueKind.String
                            ? itemNameProp.GetString()!
                            : "Неизвестный товар";

                        itemsDict[itemId] = (itemName, warehouseName);
                    }
                }
            }
        }

        var enrichedMovements = new List<Dictionary<string, object>>();

        foreach (var movement in reportDoc.RootElement.GetProperty("Movements").EnumerateArray())
        {
            var movementDict = JsonSerializer.Deserialize<Dictionary<string, object>>(movement.GetRawText())!;

            // Получаем данные из движения
            string sourceWarehouseId = movement.TryGetProperty("SourceWarehouseId", out var srcId) && srcId.ValueKind == JsonValueKind.String
                ? srcId.GetString()! : "";
            string destinationWarehouseId = movement.TryGetProperty("DestinationWarehouseId", out var destId) && destId.ValueKind == JsonValueKind.String
                ? destId.GetString()! : "";
            string itemId = movement.TryGetProperty("ItemId", out var itmId) && itmId.ValueKind == JsonValueKind.String
                ? itmId.GetString()! : "";

            // Обогащаем названиями складов
            movementDict["SourceWarehouseName"] = warehousesDict.TryGetValue(sourceWarehouseId, out var srcWarehouse) &&
                srcWarehouse.TryGetProperty("name", out var srcName) && srcName.ValueKind == JsonValueKind.String
                ? srcName.GetString()! : "Неизвестный склад";

            movementDict["DestinationWarehouseName"] = warehousesDict.TryGetValue(destinationWarehouseId, out var destWarehouse) &&
                destWarehouse.TryGetProperty("name", out var destName) && destName.ValueKind == JsonValueKind.String
                ? destName.GetString()! : "Неизвестный склад";

            // Обогащаем названием товара
            movementDict["ItemName"] = itemsDict.TryGetValue(itemId, out var itemInfo)
                ? itemInfo.itemName : "Неизвестный товар";

            // Добавляем ответственного
            var responsibleUser = await GetUserByWarehouseIdAsync(destinationWarehouseId);
            movementDict["ResponsiblePerson"] = responsibleUser ?? "Неизвестный пользователь";

            enrichedMovements.Add(movementDict);
        }

        var result = JsonSerializer.Serialize(new { Movements = enrichedMovements }, new JsonSerializerOptions { WriteIndented = true });
        _logger.LogInformation("📦 Обогащенные движения: {Result}", result);
        return result;
    }

    private async Task<string> EnrichWriteOffsReport(string reportDataJson, string stockStateJson)
    {
        using var reportDoc = JsonDocument.Parse(reportDataJson);
        using var stockDoc = JsonDocument.Parse(stockStateJson);

        // ✅ Проверяем правильную структуру ответа от InventoryService
        if (!stockDoc.RootElement.TryGetProperty("warehouses", out var warehousesElement) || warehousesElement.ValueKind != JsonValueKind.Array)
        {
            _logger.LogWarning("⚠ Ошибка: в stockStateJson отсутствует массив 'warehouses' (с маленькой буквы)");
            return reportDataJson;
        }

        // Создаем словарь складов и товаров
        var warehousesDict = new Dictionary<string, JsonElement>();
        var itemsDict = new Dictionary<string, string>(); // itemId -> itemName

        foreach (var warehouse in warehousesElement.EnumerateArray())
        {
            if (!warehouse.TryGetProperty("id", out var idProp) || idProp.ValueKind != JsonValueKind.String)
                continue;

            string warehouseId = idProp.GetString()!;
            warehousesDict[warehouseId] = warehouse;

            // Получаем товары из этого склада
            if (warehouse.TryGetProperty("items", out var itemsElement) && itemsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in itemsElement.EnumerateArray())
                {
                    if (item.TryGetProperty("id", out var itemIdProp) && itemIdProp.ValueKind == JsonValueKind.String)
                    {
                        string itemId = itemIdProp.GetString()!;
                        string itemName = item.TryGetProperty("name", out var itemNameProp) && itemNameProp.ValueKind == JsonValueKind.String
                            ? itemNameProp.GetString()!
                            : "Неизвестный товар";

                        itemsDict[itemId] = itemName;
                    }
                }
            }
        }

        var enrichedWriteOffs = new List<Dictionary<string, object>>();

        foreach (var writeOff in reportDoc.RootElement.GetProperty("WriteOffs").EnumerateArray())
        {
            var writeOffDict = JsonSerializer.Deserialize<Dictionary<string, object>>(writeOff.GetRawText())!;

            // Получаем данные из списания
            string warehouseId = writeOff.TryGetProperty("WarehouseId", out var whId) && whId.ValueKind == JsonValueKind.String
                ? whId.GetString()! : "";
            string itemId = writeOff.TryGetProperty("ItemId", out var itmId) && itmId.ValueKind == JsonValueKind.String
                ? itmId.GetString()! : "";

            // Обогащаем названием склада
            writeOffDict["WarehouseName"] = warehousesDict.TryGetValue(warehouseId, out var warehouse) &&
                warehouse.TryGetProperty("name", out var warehouseName) && warehouseName.ValueKind == JsonValueKind.String
                ? warehouseName.GetString()! : "Неизвестный склад";

            // ✅ Обогащаем названием товара (это решает ошибку "ItemName not found")
            writeOffDict["ItemName"] = itemsDict.TryGetValue(itemId, out var itemName)
                ? itemName : "Неизвестный товар";

            // Обогащаем данными пользователя
            if (writeOff.TryGetProperty("ApprovedByUser", out var approvedUserElement) && approvedUserElement.ValueKind == JsonValueKind.String)
            {
                string approvedUserId = approvedUserElement.GetString()!;
                var userName = await GetUserNameAsync(approvedUserId);
                writeOffDict["ApprovedByUser"] = userName ?? "Неизвестный пользователь";
            }
            else
            {
                writeOffDict["ApprovedByUser"] = "Не утверждено";
            }

            // Добавляем ответственного
            var responsibleUser = await GetUserByWarehouseIdAsync(warehouseId);
            writeOffDict["ResponsiblePerson"] = responsibleUser ?? "Неизвестный пользователь";

            enrichedWriteOffs.Add(writeOffDict);
        }

        var result = JsonSerializer.Serialize(new { WriteOffs = enrichedWriteOffs }, new JsonSerializerOptions { WriteIndented = true });
        _logger.LogInformation("📦 Обогащенные списания: {Result}", result);
        return result;
    }

    private async Task<string?> GetUserNameAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("⚠ Передан пустой UserId, возвращаем 'Не утверждено'");
            return "Не утверждено";
        }

        try
        {
            _logger.LogInformation("🔍 Запрос имени пользователя с эндпоинта: users/{UserId}", userId);

            var client = _httpClientFactory.CreateClient("AuthService");
            var endpoint = $"users/{userId}";

            var response = await client.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("⚠ Ошибка при получении имени пользователя {UserId}: {StatusCode}", userId, response.StatusCode);
                return "Неизвестный пользователь";
            }

            var userJson = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(userJson);

            string? firstName = jsonDoc.RootElement.TryGetProperty("firstName", out var firstNameElement) && firstNameElement.ValueKind == JsonValueKind.String
                ? firstNameElement.GetString()
                : null;

            string? lastName = jsonDoc.RootElement.TryGetProperty("lastName", out var lastNameElement) && lastNameElement.ValueKind == JsonValueKind.String
                ? lastNameElement.GetString()
                : null;

            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            {
                return $"{firstName} {lastName}";
            }

            return "Неизвестный пользователь";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Ошибка при получении имени пользователя {UserId}", userId);
            return "Неизвестный пользователь";
        }
    }

    private async Task<string?> GetUserByWarehouseIdAsync(string warehouseId)
    {
        if (string.IsNullOrEmpty(warehouseId)) return null;

        try
        {
            var client = _httpClientFactory.CreateClient("AuthService");
            var endpoint = $"users/warehouse-id/{warehouseId}";
            var response = await client.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);

            var firstName = doc.RootElement.GetProperty("firstName").GetString();
            var lastName = doc.RootElement.GetProperty("lastName").GetString();

            return $"{firstName} {lastName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Ошибка при получении пользователя по складу {WarehouseId}", warehouseId);
            return null;
        }
    }
}