using Microsoft.AspNetCore.Http;
using WriteOffService.Application.DTOs.Response.Document;
using WriteOffService.Domain.Entities;

namespace WriteOffService.Application.Interfaces.Services;

public interface IDocumentService
{
    Task<Document> CreateDocumentAsync(IFormFile file,CancellationToken cancellationToken=default);
    Task<Document> GetDocumentAsync(Guid id,CancellationToken cancellationToken=default);
    Task DeleteDocumentAsync(Guid id,CancellationToken cancellationToken=default);
    Task<DocumentInfoResponseDto> GetDocumentInfoAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Document> GetByNameAsync(string fileName, CancellationToken cancellationToken = default);

    Task<IEnumerable<DocumentInfoResponseDto>> GetByRequestIdAsync(Guid requestId,
        CancellationToken cancellationToken = default);
}