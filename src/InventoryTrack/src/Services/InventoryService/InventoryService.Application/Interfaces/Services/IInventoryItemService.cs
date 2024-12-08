using InventoryService.Application.DTOs.Request.InventoryItem;
using InventoryService.Application.DTOs.Response.InventoryItem;

namespace InventoryService.Application.Interfaces.Services;

public interface IInventoryItemService
{
    Task CreateInventoryItemAsync(CreateInventoryItemDto dto, CancellationToken cancellationToken = default);
    Task<InventoryItemResponseDto> GetInventoryItemAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryItemResponseDto>> GetInventoryItemsAsync(CancellationToken cancellationToken = default); 
    Task<IEnumerable<InventoryItemResponseDto>> GetFilteredItemsAsync(FilterInventoryItemDto filterInventoryItemDto, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryItemResponseDto>> GetInventoryItemsByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<InventoryItemResponseDto> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<InventoryItemResponseDto> GetByUniqueCodeAsync(string uniqueCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryItemResponseDto>> GetByStatusAsync(Domain.Enums.InventoryItemStatus status, CancellationToken cancellationToken = default);
    Task UpdateInventoryItemAsync(Guid id, UpdateInventoryItemDto dto, CancellationToken cancellationToken = default);
    Task DeleteInventoryItemAsync(Guid id, CancellationToken cancellationToken = default);

    Task UpdateInventoryItemStatusAsync(ChangeInventoryItemStatusDto inventoryItemStatusDto,
        CancellationToken cancellationToken = default);
}