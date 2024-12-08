using InventoryService.Domain.Entities;

namespace InventoryService.Domain.Interfaces.Repositories;

public interface ISupplierRepository:IBaseRepository<Supplier>
{
    Task<Supplier> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Supplier> GetByAccountNumber(string accountNumber, CancellationToken cancellationToken = default);
}