using WriteOffService.Domain.Entities;

namespace WriteOffService.Domain.Interfaces.Repositories;

public interface IWriteOffReasonRepository : IBaseRepository<WriteOffReason>
{
    Task<WriteOffReason> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}