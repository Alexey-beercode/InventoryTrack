using InventoryService.Application.DTOs.Request.InventoryItem;
using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace InventoryService.Application.Interfaces.Services;

public interface IInventoryItemService
{
    Task CreateInventoryItemAsync(CreateInventoryItemDto dto, CancellationToken cancellationToken = default);
    Task<InventoryItemResponseDto> GetInventoryItemAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryItemResponseDto>> GetInventoryItemsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryItemResponseDto>> GetInventoryItemsByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task MoveItemAsync(MoveItemDto moveItemDto, CancellationToken cancellationToken = default);
    Task WriteOffItemAsync(Guid itemId, Guid warehouseId, long quantity, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryItemResponseDto>> GetFilteredItemsAsync(FilterInventoryItemDto filterInventoryItemDto, CancellationToken cancellationToken = default);
    Task<InventoryItemResponseDto> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryItemResponseDto>> GetByStatusAsync(InventoryItemStatus status, CancellationToken cancellationToken = default);
    Task UpdateInventoryItemAsync(Guid id, UpdateInventoryItemDto dto, CancellationToken cancellationToken = default);
    Task DeleteInventoryItemAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateInventoryItemStatusAsync(ChangeInventoryItemStatusDto inventoryItemStatusDto, CancellationToken cancellationToken = default);
    Task AddDocumentToInventoryItemAsync(DocumentDto file, string inventoryItemName, CancellationToken cancellationToken = default);

    // ================== НОВЫЕ МЕТОДЫ ДЛЯ ПАРТИЙ ==================
    Task<IEnumerable<InventoryItemResponseDto>> GetInventoryItemsByBatchNumberAsync(string batchNumber, CancellationToken cancellationToken = default);

    Task<IEnumerable<BatchInfoDto>> GetBatchesByItemNameAsync(string itemName, Guid? warehouseId = null,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryItemResponseDto>> GetByNameAllBatchesAsync(string name, CancellationToken cancellationToken = default);
    Task WriteOffBatchAsync(string batchNumber, CancellationToken cancellationToken = default);
}