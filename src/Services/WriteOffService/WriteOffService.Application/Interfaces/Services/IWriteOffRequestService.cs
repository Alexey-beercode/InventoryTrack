using WriteOffService.Application.DTOs.Request.WriteOffRequest;
using WriteOffService.Application.DTOs.Response.WriteOffRequest;
using WriteOffService.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace WriteOffService.Application.Interfaces.Services;

public interface IWriteOffRequestService
{
    // Существующие методы
    Task<IEnumerable<WriteOffRequestResponseDto>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WriteOffRequestResponseDto>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WriteOffRequestResponseDto>> GetFilteredPagedRequestsAsync(WriteOffRequestFilterDto filterDto, CancellationToken cancellationToken = default);
    Task<IEnumerable<WriteOffRequestResponseDto>> GetByStatusAsync(RequestStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<WriteOffRequestResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WriteOffRequestResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteWriteOffRequestAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreateAsync(CreateWriteOffRequestDto createDto, CancellationToken cancellationToken = default);
    Task ApproveAsync(ApproveWriteOffRequestDto approveDto, CancellationToken cancellationToken = default);
    Task RejectAsync(Guid requestId, Guid approvedByUserId, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateWriteOffRequestDto updateWriteOffRequestDto, CancellationToken cancellationToken = default);
    Task UploadDocumentsAsync(Guid requestId, List<IFormFile> documents, CancellationToken cancellationToken = default);

    // Новые методы для работы с партиями
    Task CreateBatchWriteOffRequestAsync(CreateBatchWriteOffRequestDto createDto, CancellationToken cancellationToken = default);
}