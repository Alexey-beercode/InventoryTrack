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

    public async Task<Supplier> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(s => !s.IsDeleted && s.Name == name,cancellationToken);
    }

    public async Task<Supplier> GetByAccountNumber(string accountNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(s => !s.IsDeleted && s.AccountNumber == accountNumber,cancellationToken);
    }

    public async Task<IEnumerable<Supplier>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(s => !s.IsDeleted && s.CompanyId == companyId).ToListAsync(cancellationToken);
    }
}