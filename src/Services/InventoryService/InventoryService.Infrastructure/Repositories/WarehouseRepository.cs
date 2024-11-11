using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using InventoryService.Domain.Interfaces.Repositories;
using InventoryService.Infrastructure.Config.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;

public class WarehouseRepository : BaseRepository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(InventoryDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Warehouse>> GetByTypeAsync(WarehouseType warehouseType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(w => !w.IsDeleted && w.Type == warehouseType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Warehouse>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(w => !w.IsDeleted && w.CompanyId == companyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Warehouse>> GetByResponsiblePersonIdAsync(Guid responsiblePersonId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(w => !w.IsDeleted && w.ResponsiblePersonId == responsiblePersonId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Warehouse>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(w => !w.IsDeleted && EF.Functions.Like(w.Name, $"%{name}%"))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Warehouse>> GetByLocationAsync(string location, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(w => !w.IsDeleted && EF.Functions.Like(w.Location, $"%{location}%"))
            .ToListAsync(cancellationToken);
    }
}