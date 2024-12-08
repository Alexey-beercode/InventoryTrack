using InventoryService.Domain.Common;

namespace InventoryService.Domain.Entities;

public class InventoriesItemsWarehouses : BaseEntity
{
    public Guid ItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public long Quantity { get; set; }
    public Warehouse Warehouse { get; set; }
    public InventoryItem InventoryItem { get; set; }
}