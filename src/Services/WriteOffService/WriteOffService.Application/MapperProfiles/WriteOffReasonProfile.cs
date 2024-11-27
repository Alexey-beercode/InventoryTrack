using AutoMapper;
using WriteOffService.Application.DTOs.Response.WriteOffReason;
using WriteOffService.Domain.Entities;

namespace WriteOffService.Application.MapperProfiles;

public class WriteOffReasonProfile : Profile
{
    public WriteOffReasonProfile()
    {
        CreateMap<WriteOffReason, WriteOffReasonResponseDto>();
    }
}