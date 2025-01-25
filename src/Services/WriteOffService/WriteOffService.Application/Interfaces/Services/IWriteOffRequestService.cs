using WriteOffService.Application.DTOs.Request.WriteOffRequest;
using WriteOffService.Application.DTOs.Response.WriteOffRequest;
using WriteOffService.Domain.Enums;

namespace WriteOffService.Application.Interfaces.Services;

public interface IWriteOffRequestService
{
    Task<IEnumerable<WriteOffRequestResponseDto>> GetByWarehouseIdAsync(Guid warehouseId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<WriteOffRequestResponseDto>> GetFilteredPagedRequestsAsync(WriteOffRequestFilterDto filterDto,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<WriteOffRequestResponseDto>> GetByStatusAsync(RequestStatus status,
        CancellationToken cancellationToken = default);

    Task<WriteOffRequestResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteWriteOffRequestAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreateAsync(CreateWriteOffRequestDto createWriteOffRequestDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateWriteOffRequestDto updateWriteOffRequestDto, CancellationToken cancellationToken = default);
    Task ApproveAsync(ApproveWriteOffRequestDto approveDto, CancellationToken cancellationToken = default);
    Task RejectAsync(Guid requestId, Guid approvedByUserId, CancellationToken cancellationToken = default);
}