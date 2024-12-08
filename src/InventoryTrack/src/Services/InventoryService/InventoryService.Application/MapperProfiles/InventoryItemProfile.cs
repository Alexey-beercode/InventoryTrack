using AutoMapper;
using InventoryService.Application.DTOs.Request.InventoryItem;
using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Application.DTOs.Response.InventoryItemStatus;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;

namespace InventoryService.Application.MapperProfiles;

public class InventoryItemProfile : Profile
{
    public InventoryItemProfile()
    {
        CreateMap<CreateInventoryItemDto, InventoryItem>();
        CreateMap<UpdateInventoryItemDto, InventoryItem>();
        CreateMap<InventoryItem, InventoryItemResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapInventoryItemStatus(src.Status)));

        CreateMap<InventoryItemStatus, InventoryItemStatusResponseDto>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => GetEnumDisplayName(src)));
    }

    private InventoryItemStatusResponseDto MapInventoryItemStatus(InventoryItemStatus status)
    {
        return new InventoryItemStatusResponseDto
        {
            Value = status,
            Name = GetEnumDisplayName(status)
        };
    }

    private string GetEnumDisplayName<TEnum>(TEnum enumValue) where TEnum : Enum
    {
        return Enum.GetName(typeof(TEnum), enumValue) switch
        {
            "Requested" => "Запрошено",
            "Created" => "Создано",
            "Rejected" => "Отклонено",
            _ => enumValue.ToString(),      //Default fallback
        };
    }
}