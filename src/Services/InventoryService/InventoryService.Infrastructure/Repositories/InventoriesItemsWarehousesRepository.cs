using InventoryService.Domain.Entities;
using InventoryService.Domain.Interfaces.Repositories;
using InventoryService.Infrastructure.Config.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;

public class InventoriesItemsWarehousesRepository : BaseRepository<InventoriesItemsWarehouses>, IInventoriesItemsWarehousesRepository
{
    public InventoriesItemsWarehousesRepository(InventoryDbContext dbContext) : base(dbContext)
    {
    }
    

    public async Task<IEnumerable<InventoriesItemsWarehouses>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(iw => iw.Warehouse)
            .Include(iw => iw.InventoryItem)
            .Where(iw => !iw.IsDeleted && iw.WarehouseId == warehouseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoriesItemsWarehouses>> GetByItemIdAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(iw => iw.Warehouse)
            .Include(iw => iw.InventoryItem)
            .Where(iw => !iw.IsDeleted && iw.ItemId == itemId)
            .ToListAsync(cancellationToken);
    }
}