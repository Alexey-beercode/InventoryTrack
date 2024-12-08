using InventoryService.Domain.Entities;
using InventoryService.Domain.Interfaces.Repositories;
using InventoryService.Infrastructure.Config.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;

public class SupplierRepository : BaseRepository<Supplier>,ISupplierRepository
{
    public SupplierRepository(InventoryDbContext dbContext) : base(dbContext)
    {
    }

    public Task<Supplier> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return _dbSet.FirstOrDefaultAsync(s => !s.IsDeleted && s.Name == name,cancellationToken);
    }

    public Task<Supplier> GetByAccountNumber(string accountNumber, CancellationToken cancellationToken = default)
    {
        return _dbSet.FirstOrDefaultAsync(s => !s.IsDeleted && s.AccountNumber == accountNumber,cancellationToken);
    }
}