using AutoMapper;
using MovementService.Application.DTOs.Request.MovementRequest;
using MovementService.Application.DTOs.Response.MovementRequest;
using MovementService.Application.DTOs.Response.MovementRequestStatus;
using MovementService.Domain.Entities;
using MovementService.Domain.Enums;

namespace MovementService.Application.MapperProfiles;

public class MovementRequestProfile : Profile
{
    public MovementRequestProfile()
    {
        CreateMap<CreateMovementRequestDto, MovementRequest>();
        
        CreateMap<MovementRequest, MovementRequestResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapMovementRequestStatus(src.Status)));

        CreateMap<MovementRequestStatus, MovementRequestStatusResponseDto>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => GetEnumDisplayName(src)));
    }

    private MovementRequestStatusResponseDto MapMovementRequestStatus(MovementRequestStatus status)
    {
        return new MovementRequestStatusResponseDto
        {
            Value = status,
            Name = GetEnumDisplayName(status)
        };
    }

    private string GetEnumDisplayName<TEnum>(TEnum enumValue) where TEnum : Enum
    {
        return Enum.GetName(typeof(TEnum), enumValue) switch
        {
            "Processing" => "В обработке",
            "Rejected" => "Отклонено",
            "Completed" => "Одобрено",
            "Final"=>"Финально утверждено",
            _ => enumValue.ToString(),      //Default fallback
        };
    }
}