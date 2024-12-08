using WriteOffService.Domain.Entities;
using WriteOffService.Domain.Enums;
using WriteOffService.Domain.Models;

namespace WriteOffService.Domain.Interfaces.Repositories;

public interface IWriteOffRequestRepository : IBaseRepository<WriteOffRequest>
{
    Task<IEnumerable<WriteOffRequest>> GetByApprovedUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default);

    Task<WriteOffRequest> GetByItemIdAsync(Guid itemId, CancellationToken cancellationToken = default);

    Task<IEnumerable<WriteOffRequest>> GetByWarehouseIdAsync(Guid warehouseId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<WriteOffRequest>> GetByStatusAsync(RequestStatus status,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<WriteOffRequest>> GetFilteredAndPagedAsync(FilterWriteOffrequestModel filterModel,
        CancellationToken cancellationToken = default);

    Task<WriteOffRequest> GetByItemIdWithReasonAsync(Guid itemId, CancellationToken cancellationToken = default);

    Task<WriteOffRequest> GetByDateAndItemIdAsync(Guid itemId, DateTime date,
        CancellationToken cancellationToken = default);

}