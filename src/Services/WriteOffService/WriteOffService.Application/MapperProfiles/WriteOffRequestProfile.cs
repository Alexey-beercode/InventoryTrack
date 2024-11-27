using AutoMapper;
using WriteOffService.Application.DTOs.Request.WriteOffRequest;
using WriteOffService.Application.DTOs.Response.WriteOffRequest;
using WriteOffService.Domain.Entities;
using WriteOffService.Domain.Models;

namespace WriteOffService.Application.MapperProfiles;

public class WriteOffRequestProfile : Profile
{
    public WriteOffRequestProfile()
    {
        CreateMap<WriteOffRequest, WriteOffRequestResponseDto>()
            .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        CreateMap<CreateWriteOffRequestDto, WriteOffRequest>();
        CreateMap<WriteOffRequestFilterDto, FilterWriteOffrequestModel>();
        CreateMap<UpdateWriteOffRequestDto, WriteOffRequest>();
    }
}