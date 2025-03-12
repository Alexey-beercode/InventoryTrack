using System.Text.Json;
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

    [HttpGet("items/{id:guid}")]
    public async Task<ActionResult<BsonDocument>> GetItemDetails(Guid id, CancellationToken cancellationToken)
    {
        var item = await _inventoryItemService.GetInventoryItemAsync(id, cancellationToken);
        if (item == null) return NotFound();

        var response = new BsonDocument
        {
            { "Name", item.Name },
            { "DeliveryDate", item.DeliveryDate },
            { "ExpirationDate", item.ExpirationDate },
            { "Supplier", item.Supplier?.Name },
            { "UniqueCode", item.UniqueCode },
            { "Quantity", item.WarehouseDetails.Sum(w => w.Quantity) }
        };

        return Ok(response);
    }

    [HttpGet("warehouses/{id:guid}")]
    public async Task<ActionResult<BsonDocument>> GetWarehouseDetails(Guid id, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseService.GetByIdAsync(id, cancellationToken);
        if (warehouse == null) return NotFound();

        var response = new BsonDocument
        {
            { "Name", warehouse.Name },
            { "Location", warehouse.Location },
            { "Type", warehouse.Type?.Name }
        };

        return Ok(response);
    }
    
    [HttpGet("warehouseState/{warehouseId:guid}")]
    public async Task<ActionResult<BsonDocument>> GetWarehouseStateByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseService.GetStateByIdAsync(warehouseId, cancellationToken);
        if (warehouse == null) return NotFound();

        var response = new BsonDocument
        {
            { "Name", warehouse.Name },
            { "Location", warehouse.Location },
            { "Type", warehouse.Type?.Name }
        };

        return Ok(response);
    }

    [HttpGet("items/by-warehouse/{warehouseId:guid}")]
    public async Task<ActionResult> GetItemsByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken)
    {
        var warehouse=await _warehouseService.GetByIdAsync(warehouseId, cancellationToken);
        if (warehouse == null) return NotFound();
        var items = await _inventoryItemService.GetInventoryItemsByWarehouseAsync(warehouseId, cancellationToken);
        _logger.LogInformation("Передь методом : {items}, {warehouseId}", JsonSerializer.Serialize(items),warehouseId);
        var response = CreateDataForItems(items,warehouse);
        _logger.LogInformation("📦 JSON перед отправкой: {JsonResponse}", JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        return Ok(response);
    }

    
    [HttpGet("report-data")]
    public async Task<ActionResult> GetReportData(
        [FromQuery] string reportType, 
        [FromQuery] Guid? companyId, 
        CancellationToken cancellationToken)
    {
        BsonDocument data;
        switch (reportType.ToLower())
        {
            case "warehouses":
                if (companyId == null) return BadRequest("CompanyId is required for warehouses report.");
                data = await GetForWarehousesAsync(companyId.Value, cancellationToken);
                break;
            

            default:
                return BadRequest("Invalid report type.");
        }

        _logger.LogInformation("📦 JSON перед отправкой: {JsonResponse}", data.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.RelaxedExtendedJson }));

        return Content(data.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.RelaxedExtendedJson }));
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

private async Task<BsonDocument> GetForWarehousesAsync(Guid companyId, CancellationToken cancellationToken)
{
    var warehousesStates = await _warehouseService.GetWarehousesStatesByCompanyIdAsync(companyId, cancellationToken);
    var items = await _inventoryItemService.GetByStatusAsync(InventoryItemStatus.Created, cancellationToken);

    var warehousesArray = new BsonArray();

    foreach (var warehouse in warehousesStates)
    {
        var warehouseItems = new BsonArray();

        foreach (var item in items)
        {
            var warehouseDetails = item.WarehouseDetails
                .Where(wd => wd.WarehouseName == warehouse.Name) // Берем только текущий склад
                .ToList();

            foreach (var warehouseDetail in warehouseDetails)
            {
                warehouseItems.Add(new BsonDocument
                {
                    {"Id",item.Id.ToString()},
                    { "Name", item.Name },
                    { "UniqueCode", item.UniqueCode },
                    { "Quantity", warehouseDetail.Quantity },
                    { "EstimatedValue", item.EstimatedValue },
                    { "ExpirationDate", item.ExpirationDate },
                    { "DeliveryDate", item.DeliveryDate },
                    { "Status", item.Status?.Name ?? "Unknown" },
                    { "Supplier", item.Supplier?.Name ?? string.Empty },
                    { "WarehouseDetails", new BsonArray { new BsonDocument
                        {
                            { "WarehouseId", warehouseDetail.WarehouseId.ToString() },
                            { "WarehouseName", warehouseDetail.WarehouseName },
                            { "Quantity", warehouseDetail.Quantity }
                        }
                    }},
                    { "DocumentInfo", item.DocumentInfo != null ? item.DocumentInfo.FileName : string.Empty }
                });
            }
        }

        warehousesArray.Add(new BsonDocument
        {
            {"Id",warehouse.Id.ToString()},
            { "Name", warehouse.Name },
            { "Type", warehouse.Type?.Name ?? string.Empty },
            { "ItemsCount", warehouseItems.Count },
            { "Location", warehouse.Location },
            { "Quantity", warehouse.Quantity },
            { "Items", warehouseItems }
        });
    }

    var response = new BsonDocument { { "Warehouses", warehousesArray } };

    Console.WriteLine($"📦 Отправляемые данные из InventoryService: {response.ToJson(new JsonWriterSettings { Indent = true })}");

    return response;
}



private object CreateDataForItems(IEnumerable<InventoryItemResponseDto> inventoryItems,WarehouseResponseDto warehouse)
{
    return new
    {
        Items = inventoryItems.Select(item => new
        {
            Name = item.Name,
            UniqueCode = item.UniqueCode,
            Quantity = item.Quantity,
            EstimatedValue = item.EstimatedValue,
            ExpirationDate = item.ExpirationDate,
            DeliveryDate = item.DeliveryDate,
            Status = item.Status?.Name ?? "Unknown",
            Supplier = item.Supplier?.Name ?? string.Empty,
            WarehouseDetails = item.WarehouseDetails.Select(wd => new
            {
                WarehouseName = wd.WarehouseName,
                Quantity = wd.Quantity
            }).ToList(),
            DocumentInfo = item.DocumentInfo?.FileName ?? string.Empty
        }).ToList(),
        Warehouse=warehouse.Name
    };
}


}
