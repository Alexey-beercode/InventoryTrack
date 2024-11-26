using WriteOffService.Domain.Entities;

namespace WriteOffService.Domain.Interfaces.Repositories;

public interface IWriteOffActRepository : IBaseRepository<WriteOffAct>
{
    Task<WriteOffAct> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}