namespace InventoryService.Application.DTOs.Request.InventoryItem;

public class FilterInventoryItemDto
{
   public string Name { get; set; }
   public Guid? SupplierId {get;set;}
   public DateTime? ExpirationDateFrom { get; set; }
   public DateTime? ExpirationDateTo { get; set; }
   public decimal EstimatedValue { get; set; }
}