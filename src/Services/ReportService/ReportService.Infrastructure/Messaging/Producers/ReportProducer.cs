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
        _logger.LogInformation(jsonString);
        // Если это Movements или WriteOffs - обогащаем данные
        if (reportType == ReportType.Movements || reportType == ReportType.WriteOffs)
        {
            var stockStateData = await RequestStockStateDataAsync(companyId, dateSelect);
            return reportType == ReportType.Movements
                ? await EnrichMovementsReport(jsonString, stockStateData)
                : await EnrichWriteOffsReport(jsonString, stockStateData);
        }
        _logger.LogInformation("Финал {jsonString}",jsonString);
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
        _logger.LogInformation("📦 Полученный stockState: {StockState}", stockStateJson); // Логируем данные склада
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
                ? $"items/by-warehouse/{warehouseId}"  // ✅ Подставляем warehouseId в маршрут
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

    if (!stockDoc.RootElement.TryGetProperty("Warehouses", out var warehousesElement) || warehousesElement.ValueKind != JsonValueKind.Array)
    {
        _logger.LogWarning("⚠ Ошибка: в stockStateJson отсутствует массив 'Warehouses'");
        return reportDataJson;
    }

    var warehouses = warehousesElement.EnumerateArray().ToDictionary(
        w => w.TryGetProperty("Id", out var idProp) && idProp.ValueKind == JsonValueKind.String ? idProp.GetString()! : Guid.NewGuid().ToString(),
        w => w
    );

    var enrichedMovements = new List<Dictionary<string, object>>();

    foreach (var movement in reportDoc.RootElement.GetProperty("Movements").EnumerateArray())
    {
        var movementDict = JsonSerializer.Deserialize<Dictionary<string, object>>(movement.GetRawText())!;

        if (!movement.TryGetProperty("SourceWarehouseId", out var sourceIdElement) || sourceIdElement.ValueKind != JsonValueKind.String)
        {
            _logger.LogWarning("⚠ Пропущено перемещение: отсутствует или некорректный SourceWarehouseId");
            continue;
        }
        string sourceWarehouseId = sourceIdElement.GetString()!;

        if (!movement.TryGetProperty("DestinationWarehouseId", out var destinationIdElement) || destinationIdElement.ValueKind != JsonValueKind.String)
        {
            _logger.LogWarning("⚠ Пропущено перемещение: отсутствует или некорректный DestinationWarehouseId");
            continue;
        }
        string destinationWarehouseId = destinationIdElement.GetString()!;

        if (!movement.TryGetProperty("ItemId", out var itemIdElement) || itemIdElement.ValueKind != JsonValueKind.String)
        {
            _logger.LogWarning("⚠ Пропущено перемещение: отсутствует или некорректный ItemId");
            continue;
        }
        string itemUniqueId = itemIdElement.GetString()!;

        // 🔍 Проверка, что склад существует
        if (!warehouses.TryGetValue(sourceWarehouseId, out var sourceWarehouse) || !warehouses.TryGetValue(destinationWarehouseId, out var destinationWarehouse))
        {
            _logger.LogWarning("⚠ Перемещение пропущено: один из складов не найден: SourceId={SourceWarehouseId}, DestinationId={DestinationWarehouseId}",
                sourceWarehouseId, destinationWarehouseId);
            continue;
        }

        movementDict["SourceWarehouseName"] = sourceWarehouse.TryGetProperty("Name", out var srcName) && srcName.ValueKind == JsonValueKind.String
            ? srcName.GetString()!
            : "Неизвестный склад";

        movementDict["DestinationWarehouseName"] = destinationWarehouse.TryGetProperty("Name", out var destName) && destName.ValueKind == JsonValueKind.String
            ? destName.GetString()!
            : "Неизвестный склад";

        // 🔍 Найти товар во всех складах по `ItemId`
        var item = warehouses.Values
            .SelectMany(w => w.TryGetProperty("Items", out var items) && items.ValueKind == JsonValueKind.Array ? items.EnumerateArray() : Enumerable.Empty<JsonElement>())
            .FirstOrDefault(i => i.TryGetProperty("Id", out var id) && id.ValueKind == JsonValueKind.String && id.GetString() == itemUniqueId);

        movementDict["ItemName"] = item.ValueKind != JsonValueKind.Undefined && item.TryGetProperty("Name", out var itemName) && itemName.ValueKind == JsonValueKind.String
            ? itemName.GetString()!
            : "Неизвестный товар";
        
        var responsibleUser = await GetUserByWarehouseIdAsync(destinationWarehouseId);
        movementDict["ResponsiblePerson"] = responsibleUser ?? "Неизвестный пользователь";


        enrichedMovements.Add(movementDict);
    }

    return JsonSerializer.Serialize(new { Movements = enrichedMovements }, new JsonSerializerOptions { WriteIndented = true });
}

   private async Task<string> EnrichWriteOffsReport(string reportDataJson, string stockStateJson)
{
    using var reportDoc = JsonDocument.Parse(reportDataJson);
    using var stockDoc = JsonDocument.Parse(stockStateJson);

    if (!stockDoc.RootElement.TryGetProperty("Warehouses", out var warehousesElement) || warehousesElement.ValueKind != JsonValueKind.Array)
    {
        _logger.LogWarning("⚠ Ошибка: в stockStateJson отсутствует массив 'Warehouses'");
        return reportDataJson;
    }

    var warehouses = warehousesElement.EnumerateArray().ToDictionary(
        w => w.TryGetProperty("Id", out var idProp) && idProp.ValueKind == JsonValueKind.String ? idProp.GetString()! : Guid.NewGuid().ToString(),
        w => w
    );

    var enrichedWriteOffs = new List<Dictionary<string, object>>();

    foreach (var writeOff in reportDoc.RootElement.GetProperty("WriteOffs").EnumerateArray())
    {
        var writeOffDict = JsonSerializer.Deserialize<Dictionary<string, object>>(writeOff.GetRawText())!;

        if (!writeOff.TryGetProperty("WarehouseId", out var warehouseIdElement) || warehouseIdElement.ValueKind != JsonValueKind.String)
        {
            _logger.LogWarning("⚠ Пропущено списание: отсутствует или некорректный WarehouseId");
            continue;
        }
        string warehouseId = warehouseIdElement.GetString()!;

        if (!writeOff.TryGetProperty("ItemId", out var itemIdElement) || itemIdElement.ValueKind != JsonValueKind.String)
        {
            _logger.LogWarning("⚠ Пропущено списание: отсутствует или некорректный ItemId");
            continue;
        }
        string itemUniqueId = itemIdElement.GetString()!;

        // 🔍 Проверка, что склад существует
        if (!warehouses.TryGetValue(warehouseId, out var warehouse))
        {
            _logger.LogWarning("⚠ Списание пропущено: склад не найден: WarehouseId={WarehouseId}", warehouseId);
            continue;
        }

        writeOffDict["WarehouseName"] = warehouse.TryGetProperty("Name", out var warehouseNameElement) && warehouseNameElement.ValueKind == JsonValueKind.String
            ? warehouseNameElement.GetString()!
            : "Неизвестный склад";

        // 🔍 Найти товар на складе по `ItemId`
        var item = warehouse.TryGetProperty("Items", out var itemsElement) && itemsElement.ValueKind == JsonValueKind.Array
            ? itemsElement.EnumerateArray().FirstOrDefault(i => i.TryGetProperty("Id", out var id) && id.ValueKind == JsonValueKind.String && id.GetString() == itemUniqueId)
            : default;

        writeOffDict["ItemName"] = item.ValueKind != JsonValueKind.Undefined && item.TryGetProperty("Name", out var itemNameElement) && itemNameElement.ValueKind == JsonValueKind.String
            ? itemNameElement.GetString()!
            : "Неизвестный товар";

        // 🔍 Обогащаем `ApprovedByUser`
        string approvedByUserName = "Не утверждено"; // Значение по умолчанию

        if (writeOff.TryGetProperty("ApprovedByUser", out var approvedUserElement) && approvedUserElement.ValueKind == JsonValueKind.String)
        {
            string approvedUserId = approvedUserElement.GetString()!;
            var userName = await GetUserNameAsync(approvedUserId);

            if (!string.IsNullOrEmpty(userName))
            {
                approvedByUserName = userName;
            }
            else
            {
                _logger.LogWarning("⚠ Пользователь {ApprovedUserId} не найден, оставляем 'Неизвестный пользователь'", approvedUserId);
                approvedByUserName = "Неизвестный пользователь";
            }
        }

        writeOffDict["ApprovedByUser"] = approvedByUserName;
        
        var responsibleUser = await GetUserByWarehouseIdAsync(warehouseId);
        writeOffDict["ResponsiblePerson"] = responsibleUser ?? "Неизвестный пользователь";


        enrichedWriteOffs.Add(writeOffDict);
    }

    return JsonSerializer.Serialize(new { WriteOffs = enrichedWriteOffs }, new JsonSerializerOptions { WriteIndented = true });
}

private async Task<string?> GetUserNameAsync(string userId)
{
    if (string.IsNullOrEmpty(userId))
    {
        _logger.LogWarning("⚠ Передан пустой UserId, возвращаем 'Не утверждено'");
        return "Не утверждено";
    }

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

    // 🔍 Логируем ответ API
    _logger.LogInformation("📦 Данные пользователя: {UserJson}", jsonDoc.RootElement.GetRawText());

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
private async Task<string?> GetUserByWarehouseIdAsync(string warehouseId)
{
    if (string.IsNullOrEmpty(warehouseId)) return null;

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


}
