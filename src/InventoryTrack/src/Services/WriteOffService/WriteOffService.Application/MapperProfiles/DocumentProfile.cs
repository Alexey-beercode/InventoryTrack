using AutoMapper;
using WriteOffService.Application.DTOs.Response.Document;
using WriteOffService.Domain.Entities;

namespace WriteOffService.Application.MapperProfiles;

public class DocumentProfile:Profile
{
    public DocumentProfile()
    {
        CreateMap<Document, DocumentInfoResponseDto>();
    }
}