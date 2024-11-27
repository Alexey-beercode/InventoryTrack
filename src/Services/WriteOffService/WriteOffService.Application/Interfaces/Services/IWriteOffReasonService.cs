using WriteOffService.Application.DTOs.Response.WriteOffReason;

namespace WriteOffService.Application.Interfaces.Services;

public interface IWriteOffReasonService
{
    Task<IEnumerable<WriteOffReasonResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WriteOffReasonResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WriteOffReasonResponseDto> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task CreateAsync(string name, CancellationToken cancellationToken = default);
}