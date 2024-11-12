using AutoMapper;
using InventoryService.Application.DTOs.Request.Warehouse;
using InventoryService.Application.DTOs.Response.Warehouse;
using InventoryService.Application.DTOs.Response.WarehouseType;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;

namespace InventoryService.Application.MapperProfiles;

public class WarehouseProfile : Profile
{
    public WarehouseProfile()
    {
        CreateMap<Warehouse, WarehouseResponseDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => MapWarehouseType(src.Type)));
        
        CreateMap<WarehouseType, WarehouseTypeResponseDto>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => GetEnumDisplayName(src)));

        CreateMap<CreateWarehouseDto, Warehouse>();
    }

    private WarehouseTypeResponseDto MapWarehouseType(WarehouseType type)
    {
        return new WarehouseTypeResponseDto
        {
            Value = type,
            Name = GetEnumDisplayName(type)
        };
    }
    private string GetEnumDisplayName<TEnum>(TEnum enumValue) where TEnum : Enum
    {
        return Enum.GetName(typeof(TEnum), enumValue) switch
        {
            "Production" => "Производство", //For WarehouseType
            "Internal" => "Внутренний",     //For WarehouseType
            _ => enumValue.ToString(),      //Default fallback
        };
    }

}