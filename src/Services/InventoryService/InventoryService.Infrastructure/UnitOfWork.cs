using InventoryService.Domain.Interfaces.Repositories;
using InventoryService.Domain.Interfaces.UnitOfWork;
using InventoryService.Infrastructure.Config.Database;

namespace InventoryService.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private bool _disposed;
    private readonly InventoryDbContext _dbContext;
    private readonly IDocumentRepository _documentRepository;
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IInventoriesItemsWarehousesRepository _inventoriesItemsWarehouses;

    public UnitOfWork(InventoryDbContext dbContext, IDocumentRepository documentRepository, IInventoryItemRepository inventoryItemRepository, ISupplierRepository supplierRepository, IWarehouseRepository warehouseRepository, IInventoriesItemsWarehousesRepository inventoriesItemsWarehouses)
    {
        _dbContext = dbContext;
        _documentRepository = documentRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _supplierRepository = supplierRepository;
        _warehouseRepository = warehouseRepository;
        _inventoriesItemsWarehouses = inventoriesItemsWarehouses;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }

            _disposed = true;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken=default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public IDocumentRepository Documents => _documentRepository;
    public IInventoryItemRepository InventoryItems => _inventoryItemRepository;
    public ISupplierRepository Suppliers => _supplierRepository;
    public IWarehouseRepository Warehouses => _warehouseRepository;
    public IInventoriesItemsWarehousesRepository InventoriesItemsWarehouses => _inventoriesItemsWarehouses;
}