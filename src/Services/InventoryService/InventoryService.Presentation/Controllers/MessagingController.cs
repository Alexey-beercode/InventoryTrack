using System.Text.Json;
using BookingService.Application.Exceptions;
using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Application.DTOs.Response.Warehouse;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/inventory/messaging")]
public class MessagingController : ControllerBase
{
    private readonly IInventoryItemService _inventoryItemService;
    private readonly IWarehouseService _warehouseService;
    private readonly ILogger<MessagingController> _logger;

    public MessagingController(
        IInventoryItemService inventoryItemService,
        IWarehouseService warehouseService, ILogger<MessagingController> logger)
    {
        _inventoryItemService = inventoryItemService;
        _warehouseService = warehouseService;
        _logger = logger;
    }
    
    [HttpGet("items/by-warehouse/{warehouseId:guid}")]
    public async Task<ActionResult> GetItemsByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseService.GetByIdAsync(warehouseId, cancellationToken);
        if (warehouse == null) return NotFound();
        var items = await _inventoryItemService.GetInventoryItemsByWarehouseAsync(warehouseId, cancellationToken);
        _logger.LogInformation("Передь методом : {items}, {warehouseId}", JsonSerializer.Serialize(items), warehouseId);
        var response = CreateDataForItems(items, warehouse);
        _logger.LogInformation("📦 JSON перед отправкой: {JsonResponse}",
            JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        return Ok(response);
    }
    



    /*private async Task<BsonDocument> GetForItemsAsync(CancellationToken cancellationToken)
    {
        var items = await _inventoryItemService.GetByStatusAsync(InventoryItemStatus.Created, cancellationToken);
        return CreateDataForItems(items).ToBsonDocument();
    }

    private async Task<BsonDocument> GetForItemsRequestsAsync(CancellationToken cancellationToken)
    {
        var itemsRequests = await _inventoryItemService.GetByStatusAsync(InventoryItemStatus.Requested, cancellationToken);
        return CreateDataForItems(itemsRequests).ToBsonDocument();
    }*/

// Исправленный метод GetForWarehousesAsync без BsonDocument

// Исправленный метод CreateDataForItems в MessagingController

private object CreateDataForItems(IEnumerable<InventoryItemResponseDto> inventoryItems, WarehouseResponseDto warehouse)
{
    return new
    {
        Items = inventoryItems.Select(item => new
        {
            Id = item.Id,
            Name = item.Name,
            UniqueCode = item.UniqueCode,
            Quantity = item.Quantity,
            EstimatedValue = item.EstimatedValue,
            ExpirationDate = item.ExpirationDate,
            DeliveryDate = item.DeliveryDate,
            Status = item.Status?.Name ?? "Unknown",
            
            // ✅ ИСПРАВЛЕНО: Правильное получение поставщика и полей ТТН
            Supplier = item.Supplier?.Name ?? "Не указан", // Вместо пустой строки
            BatchNumber = !string.IsNullOrEmpty(item.BatchNumber) ? item.BatchNumber : "Не указан",
            MeasureUnit = !string.IsNullOrEmpty(item.MeasureUnit) ? item.MeasureUnit : "шт",
            VatRate = item.VatRate,
            PlacesCount = item.PlacesCount,
            CargoWeight = item.CargoWeight,
            Notes = !string.IsNullOrEmpty(item.Notes) ? item.Notes : "",
            
            WarehouseDetails = item.WarehouseDetails.Select(wd => new
            {
                WarehouseId = wd.WarehouseId,
                WarehouseName = wd.WarehouseName,
                Quantity = wd.Quantity
            }).ToList(),
            DocumentInfo = item.DocumentInfo?.FileName ?? string.Empty
        }).ToList(),
        Warehouse = warehouse.Name
    };
}

// ✅ ТАКЖЕ ИСПРАВЬТЕ метод GetForWarehousesAsync:

private async Task<object> GetForWarehousesAsync(Guid companyId, CancellationToken cancellationToken)
{
    var warehousesStates = await _warehouseService.GetWarehousesStatesByCompanyIdAsync(companyId, cancellationToken);
    var items = await _inventoryItemService.GetByStatusAsync(InventoryItemStatus.Created, cancellationToken);

    var warehouses = new List<object>();

    foreach (var warehouse in warehousesStates)
    {
        var warehouseItems = new List<object>();

        foreach (var item in items)
        {
            var warehouseDetails = item.WarehouseDetails
                .Where(wd => wd.WarehouseName == warehouse.Name)
                .ToList();

            foreach (var warehouseDetail in warehouseDetails)
            {
                warehouseItems.Add(new
                {
                    Id = item.Id.ToString(),
                    Name = item.Name,
                    UniqueCode = item.UniqueCode,
                    Quantity = warehouseDetail.Quantity,
                    EstimatedValue = item.EstimatedValue,
                    ExpirationDate = item.ExpirationDate,
                    DeliveryDate = item.DeliveryDate,
                    Status = item.Status?.Name ?? "Unknown",
                    
                    // ✅ ИСПРАВЛЕНО: Правильные значения вместо пустых строк
                    Supplier = item.Supplier?.Name ?? "Не указан",
                    BatchNumber = !string.IsNullOrEmpty(item.BatchNumber) ? item.BatchNumber : "Не указан",
                    MeasureUnit = !string.IsNullOrEmpty(item.MeasureUnit) ? item.MeasureUnit : "шт",
                    VatRate = item.VatRate,
                    PlacesCount = item.PlacesCount,
                    CargoWeight = item.CargoWeight,
                    Notes = !string.IsNullOrEmpty(item.Notes) ? item.Notes : "",
                    
                    WarehouseDetails = new[]
                    {
                        new
                        {
                            WarehouseId = warehouseDetail.WarehouseId.ToString(),
                            WarehouseName = warehouseDetail.WarehouseName,
                            Quantity = warehouseDetail.Quantity
                        }
                    },
                    DocumentInfo = item.DocumentInfo?.FileName ?? string.Empty
                });
            }
        }

        warehouses.Add(new
        {
            Id = warehouse.Id.ToString(),
            Name = warehouse.Name,
            Type = warehouse.Type?.Name ?? string.Empty,
            ItemsCount = warehouseItems.Count,
            Location = warehouse.Location,
            Quantity = warehouse.Quantity,
            Items = warehouseItems
        });
    }

    var response = new { Warehouses = warehouses };

    Console.WriteLine($"📦 Отправляемые данные из InventoryService: {JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true })}");

    return response;
}
[HttpGet("report-data")]
public async Task<ActionResult> GetReportData(
    [FromQuery] string reportType,
    [FromQuery] Guid? companyId,
    CancellationToken cancellationToken)
{
    object data;
    switch (reportType.ToLower())
    {
        case "warehouses":
            if (companyId == null) return BadRequest("CompanyId is required for warehouses report.");
            data = await GetForWarehousesAsync(companyId.Value, cancellationToken);
            break;

        default:
            return BadRequest("Invalid report type.");
    }

    _logger.LogInformation("📦 JSON перед отправкой: {JsonResponse}",
        JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));

    return Ok(data);
}

// Добавить эти методы в MessagingController

    

    [HttpPost("writeoff/batch/{batchNumber}")]
    public async Task<ActionResult> WriteOffBatchAsync(string batchNumber, CancellationToken cancellationToken)
    {
        try
        {
            await _inventoryItemService.WriteOffBatchAsync(batchNumber, cancellationToken);

            _logger.LogInformation("✅ Партия {BatchNumber} успешно списана", batchNumber);
            return Ok(new { Message = $"Партия {batchNumber} успешно списана", BatchNumber = batchNumber });
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning("❌ Партия {BatchNumber} не найдена: {Error}", batchNumber, ex.Message);
            return NotFound(new { Message = ex.Message, BatchNumber = batchNumber });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Ошибка при списании партии {BatchNumber}", batchNumber);
            return StatusCode(500, new { Message = "Внутренняя ошибка сервера", BatchNumber = batchNumber });
        }
    }

    [HttpGet("items/all-batches/{itemName}")]
    public async Task<ActionResult> GetAllBatchesByItemNameAsync(string itemName, CancellationToken cancellationToken)
    {
        var items = await _inventoryItemService.GetByNameAllBatchesAsync(itemName, cancellationToken);
        if (!items.Any()) return NotFound();

        var response = new
        {
            ItemName = itemName,
            TotalBatches = items.GroupBy(i => i.BatchNumber).Count(),
            Items = items.Select(item => new
            {
                Id = item.Id,
                Name = item.Name,
                BatchNumber = item.BatchNumber,
                UniqueCode = item.UniqueCode,
                Quantity = item.WarehouseDetails.Sum(w => w.Quantity),
                EstimatedValue = item.EstimatedValue,
                ExpirationDate = item.ExpirationDate,
                DeliveryDate = item.DeliveryDate,
                Supplier = item.Supplier?.Name ?? "",
                WarehouseDetails = item.WarehouseDetails.Select(wd => new
                {
                    WarehouseId = wd.WarehouseId,
                    WarehouseName = wd.WarehouseName,
                    Quantity = wd.Quantity
                }).ToList()
            }).ToList()
        };

        _logger.LogInformation("📦 Все партии товара {ItemName}: найдено {ItemsCount} записей", itemName,
            items.Count());
        return Ok(response);
    }
    


// Исправленный MessagingController - замена BsonDocument на обычные объекты

[HttpGet("items/by-batch/{batchNumber}")]
public async Task<ActionResult> GetItemsByBatchNumberAsync(string batchNumber,
    CancellationToken cancellationToken)
{
    _logger.LogInformation("🔍 Получен запрос на поиск партии: '{BatchNumber}'", batchNumber);
    
    var items = await _inventoryItemService.GetInventoryItemsByBatchNumberAsync(batchNumber, cancellationToken);
    
    _logger.LogInformation("🔍 Найдено товаров: {Count}", items.Count());
    
    if (!items.Any()) 
    {
        _logger.LogWarning("❌ Партия '{BatchNumber}' не найдена или пуста", batchNumber);
        return NotFound(new { Message = $"Партия '{batchNumber}' не найдена или пуста", BatchNumber = batchNumber });
    }

    var response = new
    {
        BatchNumber = batchNumber,
        Items = items.Select(item => new
        {
            Id = item.Id.ToString(),
            Name = item.Name,
            UniqueCode = item.UniqueCode,
            Quantity = item.WarehouseDetails.Sum(w => w.Quantity),
            EstimatedValue = item.EstimatedValue,
            ExpirationDate = item.ExpirationDate,
            DeliveryDate = item.DeliveryDate,
            BatchNumber = batchNumber,
            Status = item.Status?.Name ?? "Unknown",
            Supplier = item.Supplier?.Name ?? "",
            WarehouseDetails = item.WarehouseDetails.Select(wd => new
            {
                WarehouseId = wd.WarehouseId.ToString(),
                WarehouseName = wd.WarehouseName,
                Quantity = wd.Quantity
            }).ToList()
        }).ToList()
    };

    _logger.LogInformation("📦 Партия {BatchNumber}: найдено {ItemsCount} товаров", batchNumber, items.Count());
    return Ok(response);
}

[HttpGet("warehouses/{id:guid}")]
public async Task<ActionResult> GetWarehouseDetails(Guid id, CancellationToken cancellationToken)
{
    var warehouse = await _warehouseService.GetByIdAsync(id, cancellationToken);
    if (warehouse == null) return NotFound();

    var response = new
    {
        Name = warehouse.Name,
        Location = warehouse.Location,
        Type = warehouse.Type?.Name
    };

    return Ok(response);
}

[HttpGet("warehouseState/{warehouseId:guid}")]
public async Task<ActionResult> GetWarehouseStateByWarehouseIdAsync(Guid warehouseId,
    CancellationToken cancellationToken)
{
    var warehouse = await _warehouseService.GetStateByIdAsync(warehouseId, cancellationToken);
    if (warehouse == null) return NotFound();

    var response = new
    {
        Name = warehouse.Name,
        Location = warehouse.Location,
        Type = warehouse.Type?.Name
    };

    return Ok(response);
}

[HttpGet("batches/by-item/{itemName}")]
public async Task<ActionResult> GetBatchesByItemNameAsync(string itemName, Guid? warehouseId,
    CancellationToken cancellationToken)
{
    var batches = await _inventoryItemService.GetBatchesByItemNameAsync(itemName, warehouseId, cancellationToken);
    if (!batches.Any()) return NotFound();

    var response = new
    {
        ItemName = itemName,
        Batches = batches.Select(batch => new
        {
            BatchNumber = batch.BatchNumber,
            TotalQuantity = batch.TotalQuantity,
            ManufactureDate = batch.ManufactureDate,
            ExpirationDate = batch.ExpirationDate,
            ManufacturerName = batch.ManufacturerName,
            ItemsCount = batch.ItemsCount
        }).ToList()
    };

    _logger.LogInformation("📦 Товар {ItemName}: найдено {BatchesCount} партий", itemName, batches.Count());
    return Ok(response);
}

[HttpGet("items/{id:guid}")]
public async Task<ActionResult> GetItemDetails(Guid id, CancellationToken cancellationToken)
{
    var item = await _inventoryItemService.GetInventoryItemAsync(id, cancellationToken);
    if (item == null) return NotFound();

    var response = new
    {
        Name = item.Name,
        DeliveryDate = item.DeliveryDate,
        ExpirationDate = item.ExpirationDate,
        Supplier = item.Supplier?.Name,
        UniqueCode = item.UniqueCode,
        Quantity = item.WarehouseDetails.Sum(w => w.Quantity),
        
        // Новые поля
        BatchNumber = item.BatchNumber ?? "",
        MeasureUnit = item.MeasureUnit ?? "шт",
        VatRate = item.VatRate,
        PlacesCount = item.PlacesCount,
        CargoWeight = item.CargoWeight,
        Notes = item.Notes ?? ""
    };

    return Ok(response);
}
}
