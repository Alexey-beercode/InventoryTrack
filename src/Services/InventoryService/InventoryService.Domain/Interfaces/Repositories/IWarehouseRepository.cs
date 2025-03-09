using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;

namespace InventoryService.Domain.Interfaces.Repositories;

public interface IWarehouseRepository:IBaseRepository<Warehouse>
{
    Task<IEnumerable<Warehouse>> GetByTypeAsync(WarehouseType warehouseType,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Warehouse>> GetByCompanyIdAsync(Guid companyId,
        CancellationToken cancellationToken = default);

    Task<Warehouse> GetByResponsiblePersonIdAsync(Guid responsiblePersonId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Warehouse>> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<IEnumerable<Warehouse>> GetByLocationAsync(string location,
        CancellationToken cancellationToken = default);
}