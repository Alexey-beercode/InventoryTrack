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
    

    public async Task<IEnumerable<InventoriesItemsWarehouses>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(iw => iw.WarehouseId == warehouseId && !iw.IsDeleted)
            .Include(iw => iw.Warehouse) // ✅ Include Warehouse
            .Include(iw => iw.InventoryItem) // ✅ Include InventoryItem
            .ThenInclude(i => i.Supplier) // ✅ КРИТИЧНО: Include Supplier через InventoryItem
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoriesItemsWarehouses>> GetByItemIdAsync(Guid itemId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(iw => iw.ItemId == itemId && !iw.IsDeleted)
            .Include(iw => iw.Warehouse) // ✅ Include Warehouse
            .Include(iw => iw.InventoryItem) // ✅ Include InventoryItem
            .ThenInclude(i => i.Supplier) // ✅ КРИТИЧНО: Include Supplier
            .ToListAsync(cancellationToken);
    }
}