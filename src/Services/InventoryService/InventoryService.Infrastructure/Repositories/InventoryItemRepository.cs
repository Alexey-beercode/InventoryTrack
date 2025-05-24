using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using InventoryService.Domain.Interfaces.Repositories;
using InventoryService.Infrastructure.Config.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;

public class InventoryItemRepository : BaseRepository<InventoryItem>, IInventoryItemRepository
{
    public InventoryItemRepository(InventoryDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<InventoryItem>> GetFilteredItemsAsync(
        string name = null, 
        Guid? supplierId = null, 
        DateTime? expirationDateFrom = null, 
        DateTime? expirationDateTo = null, 
        decimal estimatedValue = default)
    {
        IQueryable<InventoryItem> query = _dbSet;

        if (!string.IsNullOrEmpty(name))
            query = query.Where(i => i.Name.Contains(name));

        if (supplierId.HasValue)
            query = query.Where(i => i.SupplierId == supplierId.Value);

        if (expirationDateFrom.HasValue)
            query = query.Where(i => i.ExpirationDate >= expirationDateFrom.Value);

        if (expirationDateTo.HasValue)
            query = query.Where(i => i.ExpirationDate <= expirationDateTo.Value);

        if (estimatedValue != default)
            query = query.Where(i => decimal.Abs(i.EstimatedValue - estimatedValue) <= 5);

        query = query.Where(i => i.Status == InventoryItemStatus.Created);
        return await query.AsNoTracking()
            .Include(i => i.Supplier)
            .ToListAsync();
    }
    
    public async Task<InventoryItem> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(i => i.Name == name && !i.IsDeleted)
            .Include(i => i.Supplier)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<InventoryItem> GetByUniqueCodeAsync(string uniqueCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(i => i.UniqueCode == uniqueCode && !i.IsDeleted && i.Status == InventoryItemStatus.Created)
            .Include(i => i.Supplier)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryItem>> GetByStatusAsync(InventoryItemStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => i.Status == status && !i.IsDeleted)
            .Include(i => i.Supplier)
            .ToListAsync(cancellationToken);
    }

    public async Task<InventoryItem> GetByIdCreatedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => i.Id == id && i.Status == InventoryItemStatus.Created)
            .Include(i => i.Supplier)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public new async Task<IEnumerable<InventoryItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(item => !item.IsDeleted)
            .Include(i => i.Supplier)
            .ToListAsync(cancellationToken);
    }

    public new async Task<InventoryItem> GetByIdAsync(Guid? id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Include(i => i.Supplier)
            .FirstOrDefaultAsync(cancellationToken);
    }

    // ================== НОВЫЕ МЕТОДЫ ДЛЯ ПАРТИЙ ==================

    public async Task<InventoryItem?> GetByNameAndBatchAsync(string name, string batchNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(i => i.Name == name && i.BatchNumber == batchNumber && !i.IsDeleted)
            .Include(i => i.Supplier)
            .FirstOrDefaultAsync(cancellationToken);
    }


// Обновленный метод GetByBatchNumberAsync в InventoryItemRepository
    public async Task<IEnumerable<InventoryItem>> GetByBatchNumberAsync(string batchNumber, CancellationToken cancellationToken = default)
    {
        // Добавим логирование для отладки
        Console.WriteLine($"🔍 Поиск партии: '{batchNumber}'");
    
        var query = _dbSet
            .AsNoTracking()
            .Where(i => i.BatchNumber == batchNumber && !i.IsDeleted);
    
        // Убираем фильтр по статусу - возможно, товары имеют другой статус
        // .Where(i => i.Status == InventoryItemStatus.Created)
    
        var items = await query
            .Include(i => i.Supplier)
            .ToListAsync(cancellationToken);
    
        Console.WriteLine($"🔍 Найдено товаров с партией '{batchNumber}': {items.Count}");
        foreach (var item in items)
        {
            Console.WriteLine($"   - {item.Name}, Статус: {item.Status}, BatchNumber: '{item.BatchNumber}'");
        }
    
        return items;
    }

// Альтернативный метод для поиска всех товаров с данной партией независимо от статуса
    public async Task<IEnumerable<InventoryItem>> GetByBatchNumberAllStatusesAsync(string batchNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => i.BatchNumber == batchNumber && !i.IsDeleted)
            .Include(i => i.Supplier)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryItem>> GetAllByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => i.Name == name && !i.IsDeleted && i.Status == InventoryItemStatus.Created)
            .Include(i => i.Supplier)
            .OrderByDescending(i => i.DeliveryDate) // Сортировка по дате поставки (новые партии первыми)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryItem>> GetBatchesByDatePrefixAsync(string datePrefix, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => i.BatchNumber.StartsWith(datePrefix) && !i.IsDeleted)
            .Include(i => i.Supplier)
            .ToListAsync(cancellationToken);
    }
}