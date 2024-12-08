using InventoryService.Domain.Common;
using InventoryService.Domain.Enums;

namespace InventoryService.Domain.Entities;

public class InventoryItem : BaseEntity
{
    public string Name { get; set; }
    public string UniqueCode { get; set; }
    public decimal EstimatedValue { get; set; }
    public DateTime ExpirationDate { get; set; }
    public Guid SupplierId { get; set; }
    public DateTime DeliveryDate { get; set; }
    public Guid DocumentId { get; set; }
    public InventoryItemStatus Status { get; set; }

    public Supplier Supplier { get; set; }
    public Document Document { get; set; }
}