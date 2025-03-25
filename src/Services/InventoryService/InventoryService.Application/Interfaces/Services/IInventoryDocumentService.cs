using InventoryService.Application.DTOs.Request.InventoryItem;

namespace InventoryService.Application.Interfaces.Services;

public interface IInventoryDocumentService
{
    Task<byte[]> GenerateWriteOffDocumentAsync(GenerateInventoryDocumentDto dto, CancellationToken cancellationToken);
    Task<byte[]> GenerateMovementDocumentAsync(GenerateInventoryDocumentDto dto, CancellationToken cancellationToken);
}
