using Microsoft.EntityFrameworkCore;
using WriteOffService.Domain.Entities;
using WriteOffService.Domain.Interfaces.Repositories;
using WriteOffService.Infrastructure.Config.Database;

namespace WriteOffService.Infrastructure.Repositories;

public class WriteOffActRepository : BaseRepository<WriteOffAct> , IWriteOffActRepository
{
    public WriteOffActRepository(WriteOffDbContext dbContext) : base(dbContext)
    {
    }
    
    public async Task<WriteOffAct> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.Document)
            .Include(x => x.WriteOffRequest)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
    }
}