using MovementService.Domain.Entities;
using MovementService.Domain.Enums;

namespace MovementService.Domain.Interfaces.Repositories;

public interface IMovementRequestRepository : IBaseRepository<MovementRequest>
{
    Task<IEnumerable<MovementRequest>> GetByItemIdAsync(Guid itemId, CancellationToken cancellationToken = default);

    Task<IEnumerable<MovementRequest>> GetByApprovedUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<MovementRequest>> GetBySourceWarehouseIdAsync(Guid sourceWarehouseId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<MovementRequest>> GetByDestinationWarehouseIdAsync(Guid destinationWarehouseId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<MovementRequest>> GetByStatusAsync(MovementRequestStatus status,
        CancellationToken cancellationToken = default);
}