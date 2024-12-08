using AutoMapper;
using InventoryService.Application.DTOs.Request.InventoryItem;
using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Application.DTOs.Response.InventoryItemStatus;
using InventoryService.Application.DTOs.Response.Warehouse;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;

public class InventoryItemProfile : Profile
{
    public InventoryItemProfile()
    {
        // Маппинг для InventoryItem
        CreateMap<CreateInventoryItemDto, InventoryItem>()
            .ForMember(dest => dest.UniqueCode, opt => opt.MapFrom(src => src.UniqueCode));
        CreateMap<UpdateInventoryItemDto, InventoryItem>().ReverseMap();
        CreateMap<InventoryItem, InventoryItemResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapInventoryItemStatus(src.Status)))
            .ForMember(dest => dest.Quantity, opt => opt.Ignore()); // Устанавливается в сервисе

        // Маппинг для InventoriesItemsWarehouses
        CreateMap<InventoriesItemsWarehouses, InventoryItemResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.InventoryItem.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.InventoryItem.Name))
            .ForMember(dest => dest.UniqueCode, opt => opt.MapFrom(src => src.InventoryItem.UniqueCode))
            .ForMember(dest => dest.EstimatedValue, opt => opt.MapFrom(src => src.InventoryItem.EstimatedValue))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.InventoryItem.ExpirationDate))
            .ForMember(dest => dest.DeliveryDate, opt => opt.MapFrom(src => src.InventoryItem.DeliveryDate))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapInventoryItemStatus(src.InventoryItem.Status)))
            .ForMember(dest => dest.Supplier, opt => opt.MapFrom(src => src.InventoryItem.Supplier))
            .ForMember(dest => dest.WarehouseDetails, opt => opt.MapFrom(src => src.Warehouse));

        // Маппинг для InventoryItemStatus
        CreateMap<InventoryItemStatus, InventoryItemStatusResponseDto>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => GetEnumDisplayName(src)));
        
        CreateMap<IGrouping<Guid, InventoriesItemsWarehouses>, InventoryItemResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Key))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.First().InventoryItem.Name))
            .ForMember(dest => dest.UniqueCode, opt => opt.MapFrom(src => src.First().InventoryItem.UniqueCode))
            .ForMember(dest => dest.EstimatedValue, opt => opt.MapFrom(src => src.First().InventoryItem.EstimatedValue))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.First().InventoryItem.ExpirationDate))
            .ForMember(dest => dest.Supplier, opt => opt.MapFrom(src => src.First().InventoryItem.Supplier))
            .ForMember(dest => dest.DeliveryDate, opt => opt.MapFrom(src => src.First().InventoryItem.DeliveryDate))
            .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.First().InventoryItem.DocumentId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.First().InventoryItem.Status))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Sum(iw => iw.Quantity)))
            .ForMember(dest => dest.WarehouseDetails, opt => opt.MapFrom(src => src.Select(iw => new WarehouseDetailsDto
            {
                WarehouseId = iw.WarehouseId,
                WarehouseName = iw.Warehouse.Name,
                Quantity = iw.Quantity
            })));

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
            _ => enumValue.ToString(),
        };
    }
}
