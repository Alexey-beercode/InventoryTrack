using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;

namespace InventoryService.Domain.Interfaces.Repositories;

public interface IInventoryItemRepository : IBaseRepository<InventoryItem>
{
    Task<IEnumerable<InventoryItem>> GetFilteredItemsAsync(
        string name = null, 
        Guid? supplierId = null, 
        DateTime? expirationDateFrom = null, 
        DateTime? expirationDateTo = null, 
        decimal estimatedValue = default);

    Task<InventoryItem> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<InventoryItem> GetByUniqueCodeAsync(string uniqueCode, CancellationToken cancellationToken = default);

    Task<IEnumerable<InventoryItem>> GetByStatusAsync(InventoryItemStatus status,
        CancellationToken cancellationToken = default);

    Task<InventoryItem> GetByIdCreatedAsync(Guid id, CancellationToken cancellationToken = default);
}