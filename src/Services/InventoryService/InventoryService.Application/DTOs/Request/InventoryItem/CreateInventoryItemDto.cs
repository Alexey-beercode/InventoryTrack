using Microsoft.AspNetCore.Http;

namespace InventoryService.Application.DTOs.Request.InventoryItem;

public class CreateInventoryItemDto
{
    public string Name { get; set; }
    public string UniqueCode { get; set; }
    public long Quantity { get; set; }
    public decimal EstimatedValue { get; set; }
    public DateTime ExpirationDate { get; set; }
    public Guid SupplierId { get; set; }
    public Guid WarehouseId { get; set; }
    public DateTime DeliveryDate { get; set; }
    public IFormFile DocumentFile { get; set; }
}