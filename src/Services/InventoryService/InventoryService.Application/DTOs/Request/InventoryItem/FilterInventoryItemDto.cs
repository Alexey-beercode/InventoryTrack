namespace InventoryService.Application.DTOs.Request.InventoryItem;

public class FilterInventoryItemDto
{
   public string Name { get; set; }
   public Guid? supplierId {get;set;}
   public Guid? warehouseId { get; set; }
   public DateTime? expirationDateFrom { get; set; }
   public DateTime? expirationDateTo { get; set; }
}