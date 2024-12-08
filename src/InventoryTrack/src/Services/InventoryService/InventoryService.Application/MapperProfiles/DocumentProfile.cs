using AutoMapper;
using InventoryService.Application.DTOs.Response.Document;
using InventoryService.Domain.Entities;

namespace InventoryService.Application.MapperProfiles;

public class DocumentProfile:Profile
{
    public DocumentProfile()
    {
        CreateMap<Document, DocumentInfoResponseDto>();
    }
}