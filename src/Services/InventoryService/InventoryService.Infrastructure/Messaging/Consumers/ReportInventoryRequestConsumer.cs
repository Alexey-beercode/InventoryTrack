using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Domain.Enums;
using MassTransit;
using MongoDB.Bson;

namespace InventoryService.Infrastructure.Messaging.Consumers;

public class ReportInventoryRequestConsumer : IConsumer<GetReportDataMessage>
{
    private readonly IInventoryItemService _inventoryItemService;
    private readonly IWarehouseService _warehouseService;

    public ReportInventoryRequestConsumer(IInventoryItemService inventoryItemService,
        IWarehouseService warehouseService)
    {
        _inventoryItemService = inventoryItemService;
        _warehouseService = warehouseService;
    }

    public async Task Consume(ConsumeContext<GetReportDataMessage> context)
    {
        var message = context.Message;
        var data = new BsonDocument();
        switch (message.ReportType)
        {
            case "items":
                data = await GetForItemsAsync();
                break;
            case "itemsRequests":
                data = await GetForItemsRequestsAsync();
                break;
            case "warehouses":
                data = await GetForWarehousesAsync(message.CompanyId);
                break;
        }

        await context.RespondAsync(new ReportDataResponseMessage
        {
            ReportRequestId = message.ReportRequestId,
            Data = data
        });
    }

    private async Task<BsonDocument> GetForItemsAsync()
    {
        var items = await _inventoryItemService.GetByStatusAsync(InventoryItemStatus.Created);
        return CreateDataForItems(items);
    }

    private async Task<BsonDocument> GetForItemsRequestsAsync()
    {
        var itemsRequests = await _inventoryItemService.GetByStatusAsync(InventoryItemStatus.Requested);
        return CreateDataForItems(itemsRequests);
    }

    private async Task<BsonDocument> GetForWarehousesAsync(Guid companyId)
    {
        var warehousesStates = await _warehouseService.GetWarehousesStatesByCompanyIdAsync(companyId);
        var bsonData = new BsonDocument
        {
            {
                "Warehouses", new BsonArray(warehousesStates.Select(warehouse => new BsonDocument
                {
                    { "Name", warehouse.Name },
                    { "Type", warehouse.Type?.Name ?? string.Empty },
                    { "ItemsCount", warehouse.ItemsCount },
                    { "Location", warehouse.Location },
                    { "Quantity", warehouse.Quantity },
                    {
                        "InventoryItems", new BsonArray(warehouse.InventoryItems.Select(item => new BsonDocument
                        {
                            { "Name", item.Name },
                            {
                                "Quantity",
                                item.WarehouseDetails.FirstOrDefault(w => w.WarehouseId == warehouse.Id)?.Quantity ?? 0
                            }
                        }))
                    }
                }))
            }
        };

        return bsonData;
    }

    private BsonDocument CreateDataForItems(IEnumerable<InventoryItemResponseDto> inventoryItems)
    {
        return new BsonDocument
        {
            {
                "Items", new BsonArray(inventoryItems.Select(item => new BsonDocument
                {
                    { "Name", item.Name },
                    { "UniqueCode", item.UniqueCode ?? string.Empty },
                    { "Quantity", item.Quantity },
                    { "EstimatedValue", item.EstimatedValue },
                    { "ExpirationDate", item.ExpirationDate.ToString("O") },
                    { "SupplierName", item.Supplier?.Name ?? string.Empty },
                    {
                        "WarehouseDetails", new BsonArray(item.WarehouseDetails.Select(w => new BsonDocument
                        {
                            { "WarehouseName", w.WarehouseName },
                            { "Quantity", w.Quantity }
                        }))
                    }
                }))
            }
        };
    }
}