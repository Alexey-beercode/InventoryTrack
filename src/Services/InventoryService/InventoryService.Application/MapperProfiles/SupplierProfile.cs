using AutoMapper;
using InventoryService.Application.DTOs.Request.Supplier;
using InventoryService.Application.DTOs.Response.Supplier;
using InventoryService.Domain.Entities;

namespace InventoryService.Application.MapperProfiles;

public class SupplierProfile:Profile
{
    public SupplierProfile()
    {
        CreateMap<CreateSupplierDto, Supplier>();
        CreateMap<Supplier, SupplierResponseDto>();
    }
}