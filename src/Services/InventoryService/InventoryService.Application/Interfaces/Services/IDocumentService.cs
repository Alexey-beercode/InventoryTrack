using InventoryService.Application.DTOs.Response.Document;
using InventoryService.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace InventoryService.Application.Interfaces.Services;

public interface IDocumentService
{
    Task<Document> CreateDocumentAsync(IFormFile file,CancellationToken cancellationToken=default);
    Task<Document> GetDocumentAsync(Guid id,CancellationToken cancellationToken=default);
    Task DeleteDocumentAsync(Guid id,CancellationToken cancellationToken=default);
    Task<DocumentInfoResponseDto> GetDocumentInfoAsync(Guid id, CancellationToken cancellationToken = default);
}