namespace InventoryService.Application.DTOs.Response.InventoryItemStatus;

public class InventoryItemStatusResponseDto
{
    public Domain.Enums.InventoryItemStatus Value { get; set; }
    public string Name { get; set; }
}