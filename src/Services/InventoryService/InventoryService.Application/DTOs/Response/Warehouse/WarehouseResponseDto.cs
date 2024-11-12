using InventoryService.Application.DTOs.Base;
using InventoryService.Application.DTOs.Response.WarehouseType;
using InventoryService.Domain.Enums;

namespace InventoryService.Application.DTOs.Response.Warehouse;

public class WarehouseResponseDto : BaseDto
{
    public string Name { get; set; }
    public WarehouseTypeResponseDto Type { get; set; }
    public string Location { get; set; }
    public Guid CompanyId { get; set; }
    public Guid ResponsiblePersonId { get; set; }
}