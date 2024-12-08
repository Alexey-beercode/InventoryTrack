using InventoryService.Domain.Enums;

namespace InventoryService.Application.DTOs.Request.InventoryItem;

public class UpdateInventoryItemDto
{
    public Guid WarehouseId { get; set; }
    public long Quantity { get; set; }
    public decimal EstimatedValue { get; set; }
    public InventoryItemStatus Status { get; set; }
    public string Name { get; set; }
}