using InventoryService.Domain.Enums;

namespace InventoryService.Application.DTOs.Request.InventoryItem;

public class ChangeInventoryItemStatusDto
{
    public InventoryItemStatus Status { get; set; }
    public Guid InventoryItemId { get; set; }
}