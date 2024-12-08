using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BookingService.Application.Exceptions;
using InventoryService.Application.DTOs.Response.Document;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Interfaces.UnitOfWork;
using Microsoft.AspNetCore.Http;

namespace InventoryService.Application.Services;

public class DocumentService : IDocumentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DocumentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Document> CreateDocumentAsync(IFormFile file,CancellationToken cancellationToken=default)
    {
        if (file == null || file.Length == 0)
        {
            throw new ValidationException("File is required");
        }
        
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var document = new Document
        {
            FileName = file.FileName,
            FileType = file.ContentType,
            FileData = memoryStream.ToArray()
        };

        await _unitOfWork.Documents.CreateAsync(document);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return document;
    }

    public async Task<Document> GetDocumentAsync(Guid id,CancellationToken cancellationToken=default)
    {
        var document = await _unitOfWork.Documents.GetByIdAsync(id,cancellationToken);
        if (document == null)
        {
            throw new EntityNotFoundException("Document",id);
        }
        
        return document;
    }

    public async Task DeleteDocumentAsync(Guid id,CancellationToken cancellationToken=default)
    {
        var document = await _unitOfWork.Documents.GetByIdAsync(id,cancellationToken);
        if (document == null)
        {
            throw new EntityNotFoundException("Document",id);
        }
        
        _unitOfWork.Documents.Delete(document);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<DocumentInfoResponseDto> GetDocumentInfoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var document = await _unitOfWork.Documents.GetByIdAsync(id, cancellationToken);
        if (document is null)
        {
            throw new EntityNotFoundException("Document",id);
        }

        return _mapper.Map<DocumentInfoResponseDto>(document);
    }
    
}