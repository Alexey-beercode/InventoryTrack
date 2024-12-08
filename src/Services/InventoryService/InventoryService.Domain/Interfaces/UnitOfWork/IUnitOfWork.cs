using InventoryService.Domain.Interfaces.Repositories;

namespace InventoryService.Domain.Interfaces.UnitOfWork;

public interface IUnitOfWork:IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken=default);
    IDocumentRepository Documents { get; }
    IInventoryItemRepository InventoryItems { get; }
    ISupplierRepository Suppliers { get; }
    IWarehouseRepository Warehouses { get; }
    IInventoriesItemsWarehousesRepository InventoriesItemsWarehouses { get; }
}