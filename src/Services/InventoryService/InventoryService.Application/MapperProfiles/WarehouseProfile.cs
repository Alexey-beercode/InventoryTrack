using AutoMapper;
using InventoryService.Application.DTOs.Request.Warehouse;
using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Application.DTOs.Response.Warehouse;
using InventoryService.Application.DTOs.Response.WarehouseType;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;

namespace InventoryService.Application.MapperProfiles;

public class WarehouseProfile : Profile
{
    public WarehouseProfile()
    {
        // Маппинг одного склада
        CreateMap<Warehouse, WarehouseResponseDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => MapWarehouseType(src.Type)));

        // Маппинг списка складов
        CreateMap<List<Warehouse>, List<WarehouseResponseDto>>().ConvertUsing((src, dest, context) =>
            src.Select(warehouse => context.Mapper.Map<WarehouseResponseDto>(warehouse)).ToList()
        );

        CreateMap<WarehouseType, WarehouseTypeResponseDto>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => GetEnumDisplayName(src)));

        CreateMap<CreateWarehouseDto, Warehouse>();

        CreateMap<Warehouse, WarehouseStateResponseDto>()
            .ForMember(dest => dest.ItemsCount, opt => opt.Ignore()) 
            .ForMember(dest => dest.Quantity, opt => opt.Ignore());  

        CreateMap<Warehouse, WarehouseDetailsDto>().ReverseMap();
        
        CreateMap<InventoriesItemsWarehouses, WarehouseDetailsDto>()
            .ForMember(dest => dest.WarehouseId, opt => opt.MapFrom(src => src.WarehouseId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse.Name));

        // Маппинг для InventoryItemResponseDto
        CreateMap<IGrouping<Guid, InventoriesItemsWarehouses>, InventoryItemResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Key))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.First().InventoryItem.Name))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Sum(iw => iw.Quantity)))
            .ForMember(dest => dest.WarehouseDetails, opt => opt.MapFrom(src => src));

        CreateMap<UpdateWarehouseDto, Warehouse>();
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
            "Production" => "Производственный",
            "Internal" => "Внутренний",
            _ => enumValue.ToString(),
        };
    }
}
