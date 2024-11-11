using InventoryService.Domain.Entities;
using InventoryService.Domain.Interfaces.Repositories;
using InventoryService.Infrastructure.Config.Database;

namespace InventoryService.Infrastructure.Repositories;

public class DocumentRepository : BaseRepository<Document>,IDocumentRepository
{
    public DocumentRepository(InventoryDbContext dbContext) : base(dbContext)
    {
    }
}