using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WriteOffService.Application.DTOs.Request.WriteOffRequest;
using WriteOffService.Application.Interfaces.Clients;

namespace WriteOffService.Application.Clients;

public class InventoryHttpClient : IInventoryHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InventoryHttpClient> _logger;
    private readonly string _inventoryServiceUrl;

    public InventoryHttpClient(HttpClient httpClient, IConfiguration configuration, ILogger<InventoryHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _inventoryServiceUrl = configuration["Services:InventoryService"] ?? "http://inventory-service:5111";
    }

    public async Task<BatchResponseDto?> GetItemsByBatchNumberAsync(string batchNumber,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{_inventoryServiceUrl}/api/inventory/messaging/items/by-batch/{batchNumber}";
            _logger.LogInformation("📞 Запрос к InventoryService: {Url}", url);

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("❌ Ошибка запроса к InventoryService: {StatusCode}", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("📦 Получен ответ от InventoryService: {Json}", json);

            var batchData = JsonSerializer.Deserialize<BatchResponseDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return batchData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Ошибка при обращении к InventoryService для партии {BatchNumber}", batchNumber);
            return null;
        }
    }

    public async Task<bool> WriteOffBatchAsync(string batchNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{_inventoryServiceUrl}/api/inventory/messaging/writeoff/batch/{batchNumber}";
            _logger.LogInformation("📞 Запрос на списание партии к InventoryService: {Url}", url);

            var response = await _httpClient.PostAsync(url, null, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("✅ Партия {BatchNumber} успешно списана в InventoryService", batchNumber);
                return true;
            }

            _logger.LogWarning("❌ Ошибка списания партии {BatchNumber}: {StatusCode}", batchNumber,
                response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Ошибка при списании партии {BatchNumber} в InventoryService", batchNumber);
            return false;
        }
    }
}