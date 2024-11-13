using MovementService.Domain.Interfaces.Repositories;

namespace MovementService.Domain.Interfaces.UnitOfWork;

public interface IUnitOfWork:IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken=default);
    IMovementRequestRepository MovementRequests { get; }
}