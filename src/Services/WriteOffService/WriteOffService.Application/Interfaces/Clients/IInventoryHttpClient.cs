using WriteOffService.Application.DTOs.Request.WriteOffRequest;

namespace WriteOffService.Application.Interfaces.Clients;

public interface IInventoryHttpClient
{
    Task<BatchResponseDto?> GetItemsByBatchNumberAsync(string batchNumber, CancellationToken cancellationToken = default);
    Task<bool> WriteOffBatchAsync(string batchNumber, CancellationToken cancellationToken = default);
}