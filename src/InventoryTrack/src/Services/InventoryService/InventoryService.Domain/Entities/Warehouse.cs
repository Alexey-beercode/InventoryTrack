using InventoryService.Domain.Common;
using InventoryService.Domain.Enums;

namespace InventoryService.Domain.Entities;

public class Warehouse : BaseEntity
{
    public string Name { get; set; }
    public WarehouseType Type { get; set; }
    public string Location { get; set; }
    public Guid CompanyId { get; set; }
    public Guid ResponsiblePersonId { get; set; }
}