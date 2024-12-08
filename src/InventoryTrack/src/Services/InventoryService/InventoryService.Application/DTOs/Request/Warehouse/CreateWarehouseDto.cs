using InventoryService.Domain.Enums;

namespace InventoryService.Application.DTOs.Request.Warehouse;

public class CreateWarehouseDto
{
    public string Name { get; set; }
    public WarehouseType Type { get; set; }
    public string Location { get; set; }
    public Guid CompanyId { get; set; }
    public Guid ResponsiblePersonId { get; set; }
}