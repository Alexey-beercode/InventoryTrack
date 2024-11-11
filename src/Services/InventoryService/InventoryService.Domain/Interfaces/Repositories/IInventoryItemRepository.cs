using InventoryService.Domain.Entities;

namespace InventoryService.Domain.Interfaces.Repositories;

public interface IInventoryItemRepository : IBaseRepository<InventoryItem>
{
    Task<IEnumerable<InventoryItem>> GetFilteredItemsAsync(
        string name = null,
        Guid? supplierId = null,
        Guid? warehouseId = null,
        DateTime? expirationDateFrom = null,
        DateTime? expirationDateTo = null, decimal estimatedValue = default);

    Task<IEnumerable<InventoryItem>> GetByWarehouseIdAsync(Guid warehouseId,
        CancellationToken cancellationToken = default);

    Task<InventoryItem> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<InventoryItem> GetByUniqueCodeAsync(string uniqueCode, CancellationToken cancellationToken = default);
}