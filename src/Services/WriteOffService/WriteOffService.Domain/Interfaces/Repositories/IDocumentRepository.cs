using WriteOffService.Domain.Entities;

namespace WriteOffService.Domain.Interfaces.Repositories;

public interface IDocumentRepository:IBaseRepository<Document>
{
    Task<IEnumerable<Document>> GetByWriteOffRequestIdAsync(Guid writeOffRequestId,
        CancellationToken cancellationToken = default);

    Task<Document> GetByNameAsync(string fileName, CancellationToken cancellationToken = default);
}