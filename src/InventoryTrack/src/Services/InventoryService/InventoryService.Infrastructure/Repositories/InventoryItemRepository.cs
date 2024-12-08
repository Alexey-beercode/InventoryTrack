using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using InventoryService.Domain.Interfaces.Repositories;
using InventoryService.Infrastructure.Config.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;

public class InventoryItemRepository : BaseRepository<InventoryItem>,IInventoryItemRepository
{
    public InventoryItemRepository(InventoryDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<InventoryItem>> GetFilteredItemsAsync(
        string name = null, 
        Guid? supplierId = null, 
        Guid? warehouseId = null,
        DateTime? expirationDateFrom = null, 
        DateTime? expirationDateTo = null, 
        decimal estimatedValue = default)
    {
        IQueryable<InventoryItem> query = _dbSet;

        if (!string.IsNullOrEmpty(name))
            query = query.Where(i => i.Name.Contains(name));

        if (supplierId.HasValue)
            query = query.Where(i => i.SupplierId == supplierId.Value);

        if (warehouseId.HasValue)
            query = query.Where(i => i.WarehouseId == warehouseId.Value);

        if (expirationDateFrom.HasValue)
            query = query.Where(i => i.ExpirationDate >= expirationDateFrom.Value);

        if (expirationDateTo.HasValue)
            query = query.Where(i => i.ExpirationDate <= expirationDateTo.Value);

        if (estimatedValue != default)
            query = query.Where(i => decimal.Abs(i.EstimatedValue - estimatedValue) <= 5);


        query = query.Where(i => i.Status == InventoryItemStatus.Created);
        return await query.AsNoTracking()
            .Include(i => i.Warehouse)
            .Include(i => i.Supplier)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<InventoryItem>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(e => e.WarehouseId == warehouseId && !e.IsDeleted && e.Status==InventoryItemStatus.Created)
            .Include(i => i.Warehouse)
            .Include(i => i.Supplier)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<InventoryItem> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(i => i.Name == name && !i.IsDeleted && i.Status==InventoryItemStatus.Created)
            .Include(i => i.Warehouse)
            .Include(i => i.Supplier)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<InventoryItem> GetByUniqueCodeAsync(string uniqueCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(i => i.UniqueCode == uniqueCode && !i.IsDeleted && i.Status==InventoryItemStatus.Created)
            .Include(i => i.Warehouse)
            .Include(i => i.Supplier)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryItem>> GetByStatusAsync(InventoryItemStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i=>i.Status==status && !i.IsDeleted)
            .Include(i => i.Warehouse)
            .Include(i => i.Supplier)
            .ToListAsync(cancellationToken);
    }

    public new async Task<IEnumerable<InventoryItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(item => !item.IsDeleted && item.Status==InventoryItemStatus.Created)
            .Include(i=>i.Warehouse)
            .Include(i=>i.Supplier)
            .ToListAsync(cancellationToken);
    }

    public new async Task<InventoryItem> GetByIdAsync(Guid? id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i=>i.Id==id && !i.IsDeleted && i.Status==InventoryItemStatus.Created)
            .Include(i => i.Warehouse)
            .Include(i => i.Supplier)
            .FirstOrDefaultAsync(cancellationToken);
    }
}