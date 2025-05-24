namespace InventoryService.Application.DTOs.Response.InventoryItem;

public class BatchInfoDto
{
    public string BatchNumber { get; set; }
    public long TotalQuantity { get; set; }
    public DateTime ManufactureDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string ManufacturerName { get; set; }
    public int ItemsCount { get; set; }
}