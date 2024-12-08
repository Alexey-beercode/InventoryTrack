using InventoryService.Domain.Entities;

namespace InventoryService.Domain.Interfaces.Repositories;

public interface IInventoriesItemsWarehousesRepository : IBaseRepository<InventoriesItemsWarehouses>
{
    Task<IEnumerable<InventoriesItemsWarehouses>> GetByWarehouseIdAsync(Guid warehouseId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<InventoriesItemsWarehouses>> GetByItemIdAsync(Guid itemId,
        CancellationToken cancellationToken = default);
}