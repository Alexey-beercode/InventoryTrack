using InventoryService.Domain.Enums;

namespace InventoryService.Application.DTOs.Request.Warehouse;

public class UpdateWarehouseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public WarehouseType Type { get; set; }
    public string Location { get; set; }
    public Guid CompanyId { get; set; }
    public Guid ResponsiblePersonId { get; set; }
    public Guid AccountantId { get; set; }
}