using MovementService.Application.DTOs.Request.MovementRequest;
using MovementService.Application.DTOs.Response.MovementRequest;
using MovementService.Domain.Enums;

namespace MovementService.Application.Interfaces.Services;

public interface IMovementRequestService
{
    Task CreateMovementRequestAsync(CreateMovementRequestDto dto, CancellationToken cancellationToken = default);

    Task RejectMovementRequestAsync(ChangeStatusDto changeStatusDto, CancellationToken cancellationToken = default);

    Task CompleteMovementRequestAsync(ChangeStatusDto changeStatusDto, CancellationToken cancellationToken = default);
    Task DeleteMovementRequestAsync(Guid requestId, CancellationToken cancellationToken = default);

    Task<MovementRequestResponseDto> GetMovementRequestByIdAsync(Guid requestId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<MovementRequestResponseDto>> GetMovementRequestsByItemIdAsync(Guid itemId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<MovementRequestResponseDto>> GetMovementRequestsBySourceWarehouseIdAsync(Guid sourceWarehouseId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<MovementRequestResponseDto>> GetMovementRequestsByDestinationWarehouseIdAsync(
        Guid destinationWarehouseId, CancellationToken cancellationToken = default);

    Task<IEnumerable<MovementRequestResponseDto>> GetMovementRequestsByStatusAsync(MovementRequestStatus status,
        CancellationToken cancellationToken = default);
}