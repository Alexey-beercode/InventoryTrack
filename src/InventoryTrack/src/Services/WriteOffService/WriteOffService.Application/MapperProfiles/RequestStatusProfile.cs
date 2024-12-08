using AutoMapper;
using WriteOffService.Application.DTOs.Response.RequestStatus;
using WriteOffService.Application.DTOs.Response.WriteOffRequest;
using WriteOffService.Domain.Enums;

namespace WriteOffService.Application.MapperProfiles;

public class RequestStatusProfile : Profile
{
    public RequestStatusProfile()
    {
        CreateMap<RequestStatus, RequestStatusResponseDto>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => GetEnumDisplayName(src)));
    }

    private string GetEnumDisplayName(RequestStatus status)
    {
        return status switch
        {
            RequestStatus.Requested => "Запрошено",
            RequestStatus.Created => "Создано",
            RequestStatus.Rejected => "Отклонено",
            RequestStatus.None => "Не определено",
            _ => status.ToString()
        };
    }
}