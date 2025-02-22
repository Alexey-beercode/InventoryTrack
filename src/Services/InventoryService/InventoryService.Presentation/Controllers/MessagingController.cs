using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Application.DTOs.Response.Warehouse;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

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
        IWarehouseService warehouseService,
        ILogger<MessagingController> logger)
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

    [HttpGet("report-data")]
    public async Task<ActionResult<BsonDocument>> GetReportData(
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

            case "items":
                data = await GetForItemsAsync(cancellationToken);
                break;

            case "itemsRequests":
                data = await GetForItemsRequestsAsync(cancellationToken);
                break;

            default:
                return BadRequest("Invalid report type.");
        }

        return Content(data.ToJson(), "application/json");
    }

    private async Task<BsonDocument> GetForItemsAsync(CancellationToken cancellationToken)
    {
        var items = await _inventoryItemService.GetByStatusAsync(InventoryItemStatus.Created, cancellationToken);
        return CreateDataForItems(items);
    }

    private async Task<BsonDocument> GetForItemsRequestsAsync(CancellationToken cancellationToken)
    {
        var itemsRequests = await _inventoryItemService.GetByStatusAsync(InventoryItemStatus.Requested, cancellationToken);
        return CreateDataForItems(itemsRequests);
    }

    private async Task<BsonDocument> GetForWarehousesAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var warehousesStates = await _warehouseService.GetWarehousesStatesByCompanyIdAsync(companyId, cancellationToken);
        return new BsonDocument
        {
            { "Warehouses", new BsonArray(warehousesStates.Select(warehouse => new BsonDocument
                {
                    { "Name", warehouse.Name },
                    { "Type", warehouse.Type?.Name ?? string.Empty },
                    { "ItemsCount", warehouse.ItemsCount },
                    { "Location", warehouse.Location },
                    { "Quantity", warehouse.Quantity }
                })) }
        };
    }

    private BsonDocument CreateDataForItems(IEnumerable<InventoryItemResponseDto> inventoryItems)
    {
        return new BsonDocument
        {
            {
                "Items", new BsonArray(inventoryItems.Select(item => new BsonDocument
                {
                    { "Name", item.Name },
                    { "UniqueCode", item.UniqueCode },
                    { "Quantity", item.Quantity },
                    { "EstimatedValue", item.EstimatedValue },
                    { "ExpirationDate", item.ExpirationDate },
                    { "DeliveryDate", item.DeliveryDate },
                    { "Status", item.Status?.Name ?? "Unknown" }, // Убедимся, что статус не null
                    { "Supplier", item.Supplier != null ? item.Supplier.Name : string.Empty },
                    { "WarehouseDetails", new BsonArray(item.WarehouseDetails.Select(wd => new BsonDocument
                        {
                            { "WarehouseId", wd.WarehouseName },
                            { "Quantity", wd.Quantity }
                        }))
                    },
                    { "DocumentInfo", item.DocumentInfo != null ? item.DocumentInfo.FileName : String.Empty }
                }))
            }
        };
    }

}
