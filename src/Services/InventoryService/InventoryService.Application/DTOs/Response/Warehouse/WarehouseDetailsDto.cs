namespace InventoryService.Application.DTOs.Response.Warehouse;

public class WarehouseDetailsDto
{
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; }
    public long Quantity { get; set; }
}
