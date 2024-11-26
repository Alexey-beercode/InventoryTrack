using Microsoft.EntityFrameworkCore;
using WriteOffService.Domain.Entities;
using WriteOffService.Domain.Interfaces.Repositories;
using WriteOffService.Infrastructure.Config.Database;

namespace WriteOffService.Infrastructure.Repositories;

public class DocumentRepository : BaseRepository<Document>,IDocumentRepository
{
    public DocumentRepository(WriteOffDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Document>> GetByWriteOffRequestIdAsync(Guid writeOffRequestId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WriteOffActs
            .Where(act => act.WriteOffRequestId == writeOffRequestId && !act.IsDeleted)
            .Select(act => act.Document)
            .Where(doc => !doc.IsDeleted)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}