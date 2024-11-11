using System.Reflection.Metadata;
using InventoryService.Domain.Common;

namespace InventoryService.Domain.Entities;

public class InventoryItem : BaseEntity
{
    public string Name { get; set; }
    public string UniqueCode { get; set; }
    public long Quantity { get; set; }
    public decimal EstimatedValue { get; set; }
    public DateTime ExpirationDate { get; set; }
    public Guid SupplierId { get; set; }
    public Guid WarehouseId { get; set; }
    public DateTime DeliveryDate { get; set; }
    public Guid DocumentId { get; set; }

    public Supplier Supplier { get; set; }
    public Warehouse Warehouse { get; set; }
    public Document Document { get; set; }
}