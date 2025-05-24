namespace WriteOffService.Application.DTOs.Request.WriteOffRequest;

public class WarehouseDetailDto
{
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; }
    public long Quantity { get; set; }
}