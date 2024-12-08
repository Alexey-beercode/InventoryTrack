using InventoryService.Application.DTOs.Base;
using InventoryService.Application.DTOs.Response.Document;
using InventoryService.Application.DTOs.Response.InventoryItemStatus;
using InventoryService.Application.DTOs.Response.Supplier;
using InventoryService.Application.DTOs.Response.Warehouse;
using InventoryService.Domain.Enums;

namespace InventoryService.Application.DTOs.Response.InventoryItem;

public class InventoryItemResponseDto : BaseDto
{
    public string Name { get; set; }
    public string UniqueCode { get; set; }
    public long Quantity { get; set; }
    public decimal EstimatedValue { get; set; }
    public DateTime ExpirationDate { get; set; }
    public SupplierResponseDto Supplier { get; set; }
    public DateTime DeliveryDate { get; set; }
    public Guid DocumentId { get; set; }
    public InventoryItemStatusResponseDto Status { get; set; }
    public List<WarehouseDetailsDto> WarehouseDetails { get; set; }
    public DocumentInfoResponseDto DocumentInfo { get; set; }
}
