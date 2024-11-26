using Microsoft.EntityFrameworkCore;
using WriteOffService.Domain.Entities;
using WriteOffService.Domain.Interfaces.Repositories;
using WriteOffService.Infrastructure.Config.Database;

namespace WriteOffService.Infrastructure.Repositories;

public class WriteOffReasonRepository : BaseRepository<WriteOffReason> , IWriteOffReasonRepository
{
    public WriteOffReasonRepository(WriteOffDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<WriteOffReason> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(reason => !reason.IsDeleted && reason.Reason == name,
            cancellationToken);
    }
}