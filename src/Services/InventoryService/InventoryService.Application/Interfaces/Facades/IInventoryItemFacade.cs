using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Domain.Entities;

namespace InventoryService.Application.Interfaces.Facades;

public interface IInventoryItemFacade
{
    Task<InventoryItemResponseDto> GetFullInventoryItemDto(
        IEnumerable<InventoriesItemsWarehouses> inventoriesItemsWarehouses,InventoryItem inventoryItem,
        CancellationToken cancellationToken = default);
}