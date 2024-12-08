namespace InventoryService.Application.DTOs.Request.InventoryItem;

public class MoveItemDto
{
    public Guid ItemId { get; set; }
    public Guid SourceWarehouseId { get; set; }
    public Guid DestinationWarehouseId { get; set; }
    public long Quantity { get; set; }
}